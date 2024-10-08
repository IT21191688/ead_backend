// File: ProductService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using ead_backend.Data;
using ead_backend.Model;
using ead_backend.Model.Dtos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;

namespace ead_backend.Services.ServiceImpl
{
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<Category> _categories;
        private readonly IMongoCollection<User> _users; // Add user collection for vendor details
        private readonly Cloudinary _cloudinary;

        public ProductService(MongoDbContext context, IConfiguration configuration)
        {
            _products = context.Products;
            _categories = context.Categories;
            _users = context.Users; // Initialize user collection

            var cloudinaryAccount = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(cloudinaryAccount);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto, string vendorId)
        {
            var imageUrl = await UploadImageToCloudinary(productDto.Image);

            var product = new Product
            {
                VendorId = vendorId,
                Name = productDto.Name,
                Description = productDto.Description,
                ImageUrl = imageUrl,
                Price = productDto.Price,
                Qty = productDto.Qty,
                CategoryId = productDto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _products.InsertOneAsync(product);

            return await GetProductByIdAsync(product.Id.ToString()); // Fetch full details after creation
        }

        public async Task<ProductDto> UpdateProductAsync(string productId, ProductUpdateDto productDto, string userId, string userRole)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, MongoDB.Bson.ObjectId.Parse(productId));
            var product = await _products.Find(filter).FirstOrDefaultAsync();

            if (product == null || (userRole.ToLower() != "admin" && product.VendorId != userId))
            {
                return null;
            }

            var update = Builders<Product>.Update;
            var updateDefinition = new List<UpdateDefinition<Product>>();

            if (productDto.Image != null)
            {
                var imageUrl = await UploadImageToCloudinary(productDto.Image);
                updateDefinition.Add(update.Set(p => p.ImageUrl, imageUrl));
            }

            if (productDto.Name != null)
                updateDefinition.Add(update.Set(p => p.Name, productDto.Name));

            if (productDto.Description != null)
                updateDefinition.Add(update.Set(p => p.Description, productDto.Description));

            if (productDto.Price.HasValue)
                updateDefinition.Add(update.Set(p => p.Price, productDto.Price.Value));

            if (productDto.Qty != null)
                updateDefinition.Add(update.Set(p => p.Qty, productDto.Qty));

            if (productDto.CategoryId != null)
                updateDefinition.Add(update.Set(p => p.CategoryId, productDto.CategoryId));

            if (productDto.IsActive.HasValue)
                updateDefinition.Add(update.Set(p => p.IsActive, productDto.IsActive.Value));

            updateDefinition.Add(update.Set(p => p.UpdatedAt, DateTime.UtcNow));

            var combinedUpdate = update.Combine(updateDefinition);
            await _products.UpdateOneAsync(filter, combinedUpdate);

            return await GetProductByIdAsync(productId); // Fetch updated details
        }

        public async Task<bool> DeleteProductAsync(string productId, string vendorId)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId) && p.VendorId == vendorId);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _products.Find(_ => true).ToListAsync();
            return await PopulateProductDtos(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId)).FirstOrDefaultAsync();
            if (product == null) return null;

            return await PopulateProductDto(product);
        }

        public async Task<List<ProductDto>> GetAllProductsByVendorAsync(string vendorId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.VendorId, vendorId);
            var products = await _products.Find(filter).ToListAsync();
            return await PopulateProductDtos(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(string categoryId)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.CategoryId, categoryId);
            var products = await _products.Find(filter).ToListAsync();
            return await PopulateProductDtos(products);
        }

        private async Task<ProductDto> PopulateProductDto(Product product)
        {
            var category = await _categories.Find(c => c.Id == MongoDB.Bson.ObjectId.Parse(product.CategoryId)).FirstOrDefaultAsync();
            var vendor = await _users.Find(u => u.Id == MongoDB.Bson.ObjectId.Parse(product.VendorId)).FirstOrDefaultAsync();

            return new ProductDto
            {
                Id = product.Id.ToString(),
                VendorId = product.VendorId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Qty = product.Qty,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Category = category == null ? null : new CategoryDto
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                },
                Vendor = vendor == null ? null : new UserDto
                {
                    id = vendor.Id.ToString(),
                    FirstName = vendor.FirstName,
                    Email = vendor.Email
                }
            };
        }

        private async Task<List<ProductDto>> PopulateProductDtos(List<Product> products)
        {
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var productDto = await PopulateProductDto(product);
                productDtos.Add(productDto);
            }

            return productDtos;
        }

        private async Task<string> UploadImageToCloudinary(Microsoft.AspNetCore.Http.IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            using var stream = image.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(image.FileName, stream),
                Transformation = new Transformation().Width(300).Height(300).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}

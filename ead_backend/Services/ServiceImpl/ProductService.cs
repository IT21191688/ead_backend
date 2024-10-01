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
        private readonly Cloudinary _cloudinary;

        public ProductService(MongoDbContext context, IConfiguration configuration)
        {
            _products = context.Products;
            _categories = context.Categories;

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
                UpdatedAt = product.UpdatedAt
            };
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

            var updatedProduct = await _products.Find(filter).FirstOrDefaultAsync();

            return new ProductDto
            {
                Id = updatedProduct.Id.ToString(),
                VendorId = updatedProduct.VendorId,
                Name = updatedProduct.Name,
                Description = updatedProduct.Description,
                ImageUrl = updatedProduct.ImageUrl,
                Price = updatedProduct.Price,
                Qty = updatedProduct.Qty,
                CategoryId = updatedProduct.CategoryId,
                IsActive = updatedProduct.IsActive,
                CreatedAt = updatedProduct.CreatedAt,
                UpdatedAt = updatedProduct.UpdatedAt
            };
        }

        public async Task<bool> DeleteProductAsync(string productId, string vendorId)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId) && p.VendorId == vendorId);
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _products.Find(_ => true).ToListAsync();
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var category = await _categories.Find(c => c.Id == MongoDB.Bson.ObjectId.Parse(product.CategoryId)).FirstOrDefaultAsync();

                productDtos.Add(new ProductDto
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
                    }
                });
            }

            return productDtos;
        }


        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId)).FirstOrDefaultAsync();
            if (product == null) return null;

            var category = await _categories.Find(c => c.Id == MongoDB.Bson.ObjectId.Parse(product.CategoryId)).FirstOrDefaultAsync();

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
                }
            };
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

        public async Task<List<Product>> GetAllProductsByVendorAsync(string vendorId)
        {
            // Filter products by vendorId
            var filter = Builders<Product>.Filter.Eq(p => p.VendorId, vendorId);
            var products = await _products.Find(filter).ToListAsync();

            return products;
        }
    }
}
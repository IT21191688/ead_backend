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
        private readonly Cloudinary _cloudinary;

        public ProductService(MongoDbContext context, IConfiguration configuration)
        {
            _products = context.Products;

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
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<ProductDto> UpdateProductAsync(string productId, ProductUpdateDto productDto, string userId, string userRole)
        {
            var product = await _products.Find(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId)).FirstOrDefaultAsync();

            if (product == null || (userRole.ToLower() != "admin" && product.VendorId != userId))
            {
                return null;
            }

            if (productDto.Image != null)
            {
                product.ImageUrl = await UploadImageToCloudinary(productDto.Image);
            }

            product.Name = productDto.Name ?? product.Name;
            product.Description = productDto.Description ?? product.Description;
            product.Price = productDto.Price ?? product.Price;
            product.CategoryId = productDto.CategoryId ?? product.CategoryId;
            product.IsActive = productDto.IsActive ?? product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _products.ReplaceOneAsync(p => p.Id == product.Id, product);

            return new ProductDto
            {
                Id = product.Id.ToString(),
                VendorId = product.VendorId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
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
            return products.Select(p => new ProductDto
            {
                Id = p.Id.ToString(),
                VendorId = p.VendorId,
                Name = p.Name,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CategoryId = p.CategoryId,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });
        }

        public async Task<ProductDto> GetProductByIdAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == MongoDB.Bson.ObjectId.Parse(productId)).FirstOrDefaultAsync();
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id.ToString(),
                VendorId = product.VendorId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
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
    }
}
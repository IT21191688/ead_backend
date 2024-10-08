// File: CategoryService
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

namespace ead_backend.Services.ServiceImpl
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categories;

        public CategoryService(MongoDbContext context)
        {
            _categories = context.Categories;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _categories.InsertOneAsync(category);

            return new CategoryDto
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }

        public async Task<CategoryDto> UpdateCategoryAsync(string categoryId, CategoryUpdateDto categoryDto)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, MongoDB.Bson.ObjectId.Parse(categoryId));
            var update = Builders<Category>.Update;
            var updateDefinition = new List<UpdateDefinition<Category>>();

            if (categoryDto.Name != null)
                updateDefinition.Add(update.Set(c => c.Name, categoryDto.Name));

            if (categoryDto.Description != null)
                updateDefinition.Add(update.Set(c => c.Description, categoryDto.Description));

            if (categoryDto.IsActive.HasValue)
                updateDefinition.Add(update.Set(c => c.IsActive, categoryDto.IsActive.Value));

            updateDefinition.Add(update.Set(c => c.UpdatedAt, DateTime.UtcNow));

            var combinedUpdate = update.Combine(updateDefinition);
            await _categories.UpdateOneAsync(filter, combinedUpdate);

            var updatedCategory = await _categories.Find(filter).FirstOrDefaultAsync();

            return new CategoryDto
            {
                Id = updatedCategory.Id.ToString(),
                Name = updatedCategory.Name,
                Description = updatedCategory.Description,
                IsActive = updatedCategory.IsActive,
                CreatedAt = updatedCategory.CreatedAt,
                UpdatedAt = updatedCategory.UpdatedAt
            };
        }

        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            var result = await _categories.DeleteOneAsync(c => c.Id == MongoDB.Bson.ObjectId.Parse(categoryId));
            return result.DeletedCount > 0;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categories.Find(_ => true).ToListAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(string categoryId)
        {
            var category = await _categories.Find(c => c.Id == MongoDB.Bson.ObjectId.Parse(categoryId)).FirstOrDefaultAsync();
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id.ToString(),
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
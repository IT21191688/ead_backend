using ead_backend.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ead_backend.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<CategoryDto> UpdateCategoryAsync(string categoryId, CategoryUpdateDto categoryDto);
        Task<bool> DeleteCategoryAsync(string categoryId);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(string categoryId);
    }
}
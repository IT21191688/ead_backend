// File: CategoryController
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description: category api call handling


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ead_backend.Services;
using ead_backend.Model.Dtos;
using ead_backend.Utills;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);

            if (createdCategory == null)
            {
                return this.CustomResponse(false, 400, "Failed to create category", null);
            }

            return this.CustomResponse(true, 201, "Category created successfully", createdCategory);
        }

        [HttpPut("update-category/{categoryId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(string categoryId, [FromBody] CategoryUpdateDto categoryDto)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryId, categoryDto);

            if (updatedCategory == null)
            {
                return this.CustomResponse(false, 404, "Category not found", null);
            }

            return this.CustomResponse(true, 200, "Category updated successfully", updatedCategory);
        }

        [HttpGet("get-all-category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return this.CustomResponse(true, 200, "Categories retrieved successfully", categories);
        }

        [HttpGet("get-one-category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(string categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category == null)
            {
                return this.CustomResponse(false, 404, "Category not found", null);
            }

            return this.CustomResponse(true, 200, "Category retrieved successfully", category);
        }

        [HttpDelete("delete-category/{categoryId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(string categoryId)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            if (!result)
            {
                return this.CustomResponse(false, 404, "Category not found", null);
            }

            return this.CustomResponse(true, 200, "Category deleted successfully", null);
        }
    }
}
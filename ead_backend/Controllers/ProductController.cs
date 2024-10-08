// File: ProductController
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description: Handle All Product apis

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using ead_backend.Services;
using ead_backend.Model.Dtos;
using ead_backend.Utills;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IUserService userService)
        {
            _productService = productService;
            _userService = userService;
        }


        [HttpPost("create-product")]
        [Authorize(Policy = "VendorOrAdmin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto productDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null || (user.Role.ToLower() != "vendor" && user.Role.ToLower() != "admin"))
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var createdProduct = await _productService.CreateProductAsync(productDto, user.Id.ToString());

            if (createdProduct == null)
            {
                return this.CustomResponse(false, 400, "Failed to create product", null);
            }

            return this.CustomResponse(true, 201, "Product created successfully", createdProduct);
        }
        [HttpPut("update-product/{productId}")]
        [Authorize(Policy = "VendorOrAdmin")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromForm] ProductUpdateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return this.CustomResponse(false, 400, "Invalid input", ModelState);
            }

            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null || (user.Role.ToLower() != "vendor" && user.Role.ToLower() != "admin"))
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var updatedProduct = await _productService.UpdateProductAsync(productId, productDto, user.Id.ToString(), user.Role);

            if (updatedProduct == null)
            {
                return this.CustomResponse(false, 404, "Product not found or you don't have permission to update it", null);
            }

            return this.CustomResponse(true, 200, "Product updated successfully", updatedProduct);
        }

        [HttpDelete("delete-product/{productId}")]
        [Authorize(Policy = "VendorOrAdmin")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            var vendorEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var vendor = await _userService.GetUserByEmailAsync(vendorEmail);

            if (vendor == null || vendor.Role.ToLower() != "vendor")
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var result = await _productService.DeleteProductAsync(productId, vendor.Id.ToString());

            if (!result)
            {
                return this.CustomResponse(false, 404, "Product not found or you don't have permission to delete it", null);
            }

            return this.CustomResponse(true, 200, "Product deleted successfully", null);
        }

        [HttpGet("get-all-products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return this.CustomResponse(true, 200, "Products retrieved successfully", products);
        }

        [HttpGet("get-all-products-by-vendor")]
        [Authorize(Roles = "vendor")]
        public async Task<IActionResult> GetAllProductsByVendor()
        { 
            var vendorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (vendorIdClaim == null)
            {
                return this.CustomResponse(false, 400, "Vendor ID not found in token", null);
            }

            var vendorId = vendorIdClaim.Value;

            var products = await _productService.GetAllProductsByVendorAsync(vendorId);

            if (products == null || !products.Any())
            {
                return this.CustomResponse(false, 404, "No products found for the vendor", null);
            }

            return this.CustomResponse(true, 200, "Products retrieved successfully", products);
        }

        [HttpGet("get-one-product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(string productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            if (product == null)
            {
                return this.CustomResponse(false, 404, "Product not found", null);
            }

            return this.CustomResponse(true, 200, "Product retrieved successfully", product);
        }
    }
}
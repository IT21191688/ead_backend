using ead_backend.Model.Dtos;
using ead_backend.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ead_backend.Services
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(ProductCreateDto productDto, string vendorId);
        Task<ProductDto> UpdateProductAsync(string productId, ProductUpdateDto productDto, string userId, string userRole);
        Task<bool> DeleteProductAsync(string productId, string vendorId);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(string productId);

        Task<List<ProductDto>> GetAllProductsByVendorAsync(string vendorId);
    }
}
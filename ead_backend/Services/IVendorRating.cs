// File: IVendorRatingService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using ead_backend.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ead_backend.Services
{
    public interface IVendorRatingService
    {
        Task<VendorRatingDto> CreateVendorRatingAsync(string customerId, VendorRatingCreateDto vendorRatingCreateDto);
        Task<IEnumerable<VendorRatingDto>> GetRatingsByVendorIdAsync(string vendorId);
        Task<IEnumerable<VendorRatingDto>> GetRatingsByCustomerIdAsync(string customerId);
        Task<VendorRatingDto> GetRatingByIdAsync(string ratingId);
        Task<IEnumerable<VendorRatingDto>> GetAllRatingsAsync();
    }
}

using ead_backend.Model.Dtos;
using ead_backend.Services;
using ead_backend.Utills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/vendor-ratings")]
    [Authorize]
    public class VendorRatingController : ControllerBase
    {
        private readonly IVendorRatingService _vendorRatingService;
        private readonly IUserService _userService;

        public VendorRatingController(IVendorRatingService vendorRatingService, IUserService userService)
        {
            _vendorRatingService = vendorRatingService;
            _userService = userService;
        }

        [HttpPost("create-vendor-rating")]
        public async Task<IActionResult> CreateVendorRating([FromBody] VendorRatingCreateDto vendorRatingCreateDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            // Ensure that the vendor exists and is a vendor
            var vendor = await _userService.GetUserByIdAsync(vendorRatingCreateDto.VendorId);
            if (vendor == null || vendor.Role.ToLower() != "vendor")
            {
                return this.CustomResponse(false, 400, "Invalid vendor ID", null);
            }

            var rating = await _vendorRatingService.CreateVendorRatingAsync(user.Id.ToString(), vendorRatingCreateDto);
            return this.CustomResponse(true, 201, "Vendor rating created successfully", rating);
        }

        [HttpGet("vendor-rating-get-by-vendor-id/{vendorId}")]
        public async Task<IActionResult> GetRatingsByVendorId(string vendorId)
        {
            var ratings = await _vendorRatingService.GetRatingsByVendorIdAsync(vendorId);
            return this.CustomResponse(true, 200, "Vendor ratings retrieved successfully", ratings);
        }

        [HttpGet("all-ratings")]
        public async Task<IActionResult> GetAllRatings()
        {
            var ratings = await _vendorRatingService.GetAllRatingsAsync();
            return this.CustomResponse(true, 200, "All vendor ratings retrieved successfully", ratings);
        }

        [HttpGet("vendor-rating-by-customer-id/{customerId}")]
        public async Task<IActionResult> GetRatingsByCustomerId(string customerId)
        {
            var ratings = await _vendorRatingService.GetRatingsByCustomerIdAsync(customerId);
            return this.CustomResponse(true, 200, "Customer ratings retrieved successfully", ratings);
        }

        [HttpGet("vendor-rating-by-id/{ratingId}")]
        public async Task<IActionResult> GetRatingById(string ratingId)
        {
            var rating = await _vendorRatingService.GetRatingByIdAsync(ratingId);
            if (rating == null)
            {
                return this.CustomResponse(false, 404, "Rating not found", null);
            }

            return this.CustomResponse(true, 200, "Rating retrieved successfully", rating);
        }
    }
}

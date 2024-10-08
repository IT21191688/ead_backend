// File: VendorRatingDto
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:
namespace ead_backend.Model.Dtos
{
    public class VendorRatingDto
    {
        public string Id { get; set; }
        public string VendorId { get; set; }
        public string CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto Vendor { get; set; }  // Assuming there's a UserDto for vendor
        public UserDto Customer { get; set; }  // Assuming there's a UserDto for customer
    }


    public class VendorRatingCreateDto
    {
        public string VendorId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}

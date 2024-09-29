//using ead_backend.Data;
//using ead_backend.Model;
//using ead_backend.Model.Dtos;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace ead_backend.Services.ServiceImpl
//{
//    public class VendorRatingService : IVendorRatingService
//    {
//        private readonly IMongoCollection<VendorRating> _vendorRatings;
//        private readonly IMongoCollection<User> _users;

//        public VendorRatingService(MongoDbContext context)
//        {
//            _vendorRatings = context.VendorRatings;
//            _users = context.Users;
//        }

//        public async Task<VendorRatingDto> CreateVendorRatingAsync(string customerId, VendorRatingCreateDto vendorRatingCreateDto)
//        {
//            var vendorRating = new VendorRating
//            {
//                Id = ObjectId.GenerateNewId(),
//                VendorId = vendorRatingCreateDto.VendorId,
//                CustomerId = customerId,
//                Rating = vendorRatingCreateDto.Rating,
//                Comment = vendorRatingCreateDto.Comment,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _vendorRatings.InsertOneAsync(vendorRating);

//            return await GetRatingByIdAsync(vendorRating.Id.ToString());
//        }

//        public async Task<IEnumerable<VendorRatingDto>> GetRatingsByVendorIdAsync(string vendorId)
//        {
//            var ratings = await _vendorRatings.Find(vr => vr.VendorId == vendorId).ToListAsync();
//            var ratingDtos = new List<VendorRatingDto>();

//            foreach (var rating in ratings)
//            {
//                ratingDtos.Add(await MapVendorRatingToDtoAsync(rating));
//            }

//            return ratingDtos;
//        }

//        public async Task<IEnumerable<VendorRatingDto>> GetRatingsByCustomerIdAsync(string customerId)
//        {
//            var ratings = await _vendorRatings.Find(vr => vr.CustomerId == customerId).ToListAsync();
//            var ratingDtos = new List<VendorRatingDto>();

//            foreach (var rating in ratings)
//            {
//                ratingDtos.Add(await MapVendorRatingToDtoAsync(rating));
//            }

//            return ratingDtos;
//        }

//        public async Task<VendorRatingDto> GetRatingByIdAsync(string ratingId)
//        {
//            if (!ObjectId.TryParse(ratingId, out ObjectId ratingObjectId))
//            {
//                return null;
//            }

//            var rating = await _vendorRatings.Find(vr => vr.Id == ratingObjectId).FirstOrDefaultAsync();
//            if (rating == null)
//            {
//                return null;
//            }

//            return await MapVendorRatingToDtoAsync(rating);
//        }

//        private async Task<VendorRatingDto> MapVendorRatingToDtoAsync(VendorRating rating)
//        {
//            var vendorTask = _users.Find(u => u.Id.ToString() == rating.VendorId).FirstOrDefaultAsync();
//            var customerTask = _users.Find(u => u.Id.ToString() == rating.CustomerId).FirstOrDefaultAsync();

//            await Task.WhenAll(vendorTask, customerTask);

//            var vendor = vendorTask.Result;
//            var customer = customerTask.Result;

//            return new VendorRatingDto
//            {
//                Id = rating.Id.ToString(),
//                VendorId = rating.VendorId,
//                CustomerId = rating.CustomerId,
//                Rating = rating.Rating,
//                Comment = rating.Comment,
//                CreatedAt = rating.CreatedAt,
//                Vendor = MapUserToDto(vendor),
//                Customer = MapUserToDto(customer)
//            };
//        }

//        public async Task<IEnumerable<VendorRatingDto>> GetAllRatingsAsync()
//{
//    var ratings = await _vendorRatings.Find(_ => true).ToListAsync();

//    var vendorRatingDtos = new List<VendorRatingDto>();

//    foreach (var rating in ratings)
//    {
//        var vendor = await _users.(rating.VendorId);
//        var customer = await _userService.GetUserByIdAsync(rating.CustomerId);

//        var vendorRatingDto = new VendorRatingDto
//        {
//            Id = rating.Id.ToString(),
//            VendorId = rating.VendorId,
//            CustomerId = rating.CustomerId,
//            Rating = rating.Rating,
//            Comment = rating.Comment,
//            CreatedAt = rating.CreatedAt ?? DateTime.MinValue,
//            Vendor = vendor != null ? new UserDto
//            {
//                FirstName = vendor.FirstName,
//                LastName = vendor.LastName,
//                Email = vendor.Email,
//                Age = vendor.Age,
//                Role = vendor.Role,
//                Status = vendor.Status
//            } : null,
//            Customer = customer != null ? new UserDto
//            {
//                FirstName = customer.FirstName,
//                LastName = customer.LastName,
//                Email = customer.Email,
//                Age = customer.Age,
//                Role = customer.Role,
//                Status = customer.Status
//            } : null
//        };

//        vendorRatingDtos.Add(vendorRatingDto);
//    }

//    return vendorRatingDtos;
//}


//        private UserDto MapUserToDto(User user)
//        {
//            if (user == null) return null;

//            return new UserDto
//            {
//                //Id = user.ToString(),
//                FirstName = user.FirstName,
//                LastName = user.LastName,
//                Age = user.Age,
//                Email = user.Email,
//                Role = user.Role
//            };
//        }
//    }
//}

using ead_backend.Data;
using ead_backend.Model;
using ead_backend.Model.Dtos;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ead_backend.Services.ServiceImpl
{
    public class VendorRatingService : IVendorRatingService
    {
        private readonly IMongoCollection<VendorRating> _vendorRatings;
        private readonly IMongoCollection<User> _users;

        public VendorRatingService(MongoDbContext context)
        {
            _vendorRatings = context.VendorRatings;
            _users = context.Users;
        }

        public async Task<VendorRatingDto> CreateVendorRatingAsync(string customerId, VendorRatingCreateDto vendorRatingCreateDto)
        {
            var vendorRating = new VendorRating
            {
                Id = ObjectId.GenerateNewId(),
                VendorId = vendorRatingCreateDto.VendorId,
                CustomerId = customerId,
                Rating = vendorRatingCreateDto.Rating,
                Comment = vendorRatingCreateDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _vendorRatings.InsertOneAsync(vendorRating);
            return await GetRatingByIdAsync(vendorRating.Id.ToString());
        }

        public async Task<IEnumerable<VendorRatingDto>> GetRatingsByVendorIdAsync(string vendorId)
        {
            var ratings = await _vendorRatings.Find(vr => vr.VendorId == vendorId).ToListAsync();
            var ratingDtos = new List<VendorRatingDto>();

            foreach (var rating in ratings)
            {
                ratingDtos.Add(await MapVendorRatingToDtoAsync(rating));
            }

            return ratingDtos;
        }

        public async Task<IEnumerable<VendorRatingDto>> GetRatingsByCustomerIdAsync(string customerId)
        {
            var ratings = await _vendorRatings.Find(vr => vr.CustomerId == customerId).ToListAsync();
            var ratingDtos = new List<VendorRatingDto>();

            foreach (var rating in ratings)
            {
                ratingDtos.Add(await MapVendorRatingToDtoAsync(rating));
            }

            return ratingDtos;
        }

        public async Task<VendorRatingDto> GetRatingByIdAsync(string ratingId)
        {
            if (!ObjectId.TryParse(ratingId, out ObjectId ratingObjectId))
            {
                return null;
            }

            var rating = await _vendorRatings.Find(vr => vr.Id == ratingObjectId).FirstOrDefaultAsync();
            if (rating == null)
            {
                return null;
            }

            return await MapVendorRatingToDtoAsync(rating);
        }

        public async Task<IEnumerable<VendorRatingDto>> GetAllRatingsAsync()
        {
            var ratings = await _vendorRatings.Find(_ => true).ToListAsync();
            var vendorRatingDtos = new List<VendorRatingDto>();

            foreach (var rating in ratings)
            {
                var vendor = await _users.Find(u => u.Id.ToString() == rating.VendorId).FirstOrDefaultAsync();
                var customer = await _users.Find(u => u.Id.ToString() == rating.CustomerId).FirstOrDefaultAsync();

                var vendorRatingDto = new VendorRatingDto
                {
                    Id = rating.Id.ToString(),
                    VendorId = rating.VendorId,
                    CustomerId = rating.CustomerId,
                    Rating = rating.Rating,
                    Comment = rating.Comment,
                    CreatedAt = DateTime.MinValue,
                    Vendor = vendor != null ? new UserDto
                    {
                        FirstName = vendor.FirstName,
                        LastName = vendor.LastName,
                        Email = vendor.Email,
                        Age = vendor.Age,
                        Role = vendor.Role,
                        Status = vendor.Status
                    } : null,
                    Customer = customer != null ? new UserDto
                    {
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        Email = customer.Email,
                        Age = customer.Age,
                        Role = customer.Role,
                        Status = customer.Status
                    } : null
                };

                vendorRatingDtos.Add(vendorRatingDto);
            }

            return vendorRatingDtos;
        }

        private async Task<VendorRatingDto> MapVendorRatingToDtoAsync(VendorRating rating)
        {
            var vendorTask = _users.Find(u => u.Id.ToString() == rating.VendorId).FirstOrDefaultAsync();
            var customerTask = _users.Find(u => u.Id.ToString() == rating.CustomerId).FirstOrDefaultAsync();

            await Task.WhenAll(vendorTask, customerTask);

            var vendor = vendorTask.Result;
            var customer = customerTask.Result;

            return new VendorRatingDto
            {
                Id = rating.Id.ToString(),
                VendorId = rating.VendorId,
                CustomerId = rating.CustomerId,
                Rating = rating.Rating,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt,
                Vendor = MapUserToDto(vendor),
                Customer = MapUserToDto(customer)
            };
        }

        private UserDto MapUserToDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}

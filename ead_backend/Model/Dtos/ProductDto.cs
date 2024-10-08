﻿// File: ProductDto
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:
using Microsoft.AspNetCore.Http;
using System;

namespace ead_backend.Model.Dtos
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string VendorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public string CategoryId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CategoryDto Category { get; set; } // Include category details
        public UserDto Vendor { get; set; }
    }

    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public decimal Price { get; set; }

        public int Qty { get; set; }
        public string CategoryId { get; set; }
    }

    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public decimal? Price { get; set; }

        public int Qty { get; set; }

        public string? CategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
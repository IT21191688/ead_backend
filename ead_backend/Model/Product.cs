// File: Product
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ead_backend.Model
{
    public class Product
    {
        [BsonId]
        public ObjectId Id { get; set; }
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
    }
}
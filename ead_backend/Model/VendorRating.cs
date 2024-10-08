// File: VendorRating
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ead_backend.Model
{
    public class VendorRating
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string VendorId { get; set; } // FK to Users
        public string CustomerId { get; set; } // FK to Users
        public int Rating { get; set; } // Assuming rating is an integer (e.g., 1-5)
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

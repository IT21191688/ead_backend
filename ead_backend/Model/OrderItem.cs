using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ead_backend.Model
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }

        public string Status { get; set; } // Processing, Delivered

        [BsonIgnore]
        public Product Product { get; set; }

        [BsonIgnore]
        public User Vendor { get; set; }
    }
}
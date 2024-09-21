using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ead_backend.Model
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
    }
}

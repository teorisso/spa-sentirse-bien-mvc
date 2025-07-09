using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpaAdmin.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("last_name")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonIgnore]
        public string FullName => $"{FirstName} {LastName}";
    }
}
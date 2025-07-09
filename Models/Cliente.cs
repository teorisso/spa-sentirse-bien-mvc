using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpaAdmin.Models
{
    [BsonIgnoreExtraElements]
    public class Cliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("first_name")]
        public string? FirstName { get; set; }

        [BsonElement("last_name")]
        public string? LastName { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("role")]
        public string? Role { get; set; }
    }
}
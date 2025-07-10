using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SpaAdmin.Models
{
    [BsonIgnoreExtraElements]
    public class Turno
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("cliente")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClienteId { get; set; } = string.Empty;

        [BsonElement("servicio")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ServicioId { get; set; } = string.Empty;

        [BsonElement("profesional")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProfesionalId { get; set; } = string.Empty;

        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }

        [BsonElement("hora")]
        public string Hora { get; set; } = string.Empty;

        [BsonElement("estado")]
        public string Estado { get; set; } = "pendiente";

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedades de navegación (no se guardan en BD)
        [BsonIgnore]
        public Cliente? Cliente { get; set; }

        [BsonIgnore]
        public Servicio? Servicio { get; set; }

        [BsonIgnore]
        public Profesional? Profesional { get; set; }
    }
}
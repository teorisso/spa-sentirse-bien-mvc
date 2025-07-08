using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SpaAdmin.Models
{
    [BsonIgnoreExtraElements] // Ignorar campos adicionales como __v de Mongoose
    public class Servicio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("precio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal? Precio { get; set; }

        [BsonElement("tipo")]
        public string? Tipo { get; set; }

        [BsonElement("descripcion")]
        public string? Descripcion { get; set; }

        [BsonElement("Image")]
        public string? Imagen { get; set; }

        // Propiedades adicionales para el MVC (no mapeadas a MongoDB)
        [BsonIgnore]
        [Required(ErrorMessage = "La categoría es requerida")]
        public string Categoria 
        { 
            get => Tipo ?? string.Empty; 
            set => Tipo = value; 
        }

        [BsonIgnore]
        [Required(ErrorMessage = "La duración es requerida")]
        [Range(1, 480, ErrorMessage = "La duración debe estar entre 1 y 480 minutos")]
        public int DuracionMinutos { get; set; } = 60; // Valor por defecto
    }
}




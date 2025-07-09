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
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Nombre del Servicio")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("precio")]
        [Range(0.01, 9999.99, ErrorMessage = "El precio debe estar entre $0.01 y $9999.99")]
        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public decimal? Precio { get; set; }

        [BsonElement("tipo")]
        [Required(ErrorMessage = "El tipo de servicio es requerido")]
        [StringLength(50, ErrorMessage = "El tipo no puede tener más de 50 caracteres")]
        [Display(Name = "Tipo de Servicio")]
        public string? Tipo { get; set; }

        [BsonElement("descripcion")]
        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [BsonElement("Image")]
        [Display(Name = "Imagen")]
        public string? Imagen { get; set; }

        // Propiedades adicionales para el MVC (no mapeadas a MongoDB)
        [BsonIgnore]
        [Required(ErrorMessage = "La categoría es requerida")]
        [Display(Name = "Categoría")]
        public string Categoria 
        { 
            get => Tipo ?? string.Empty; 
            set => Tipo = value; 
        }

        [BsonIgnore]
        [Required(ErrorMessage = "La duración es requerida")]
        [Range(15, 480, ErrorMessage = "La duración debe estar entre 15 y 480 minutos")]
        [Display(Name = "Duración (minutos)")]
        public int DuracionMinutos { get; set; } = 60; // Valor por defecto

        [BsonIgnore]
        [Display(Name = "Duración Formateada")]
        public string DuracionFormateada
        {
            get
            {
                var horas = DuracionMinutos / 60;
                var minutos = DuracionMinutos % 60;
                return horas > 0 ? $"{horas}h {minutos}min" : $"{minutos}min";
            }
        }
    }
}




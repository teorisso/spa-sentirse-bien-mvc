using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SpaAdmin.Models
{
    [BsonIgnoreExtraElements]
    public class Cliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("first_name")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre")]
        public string? FirstName { get; set; }

        [BsonElement("last_name")]
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres")]
        [Display(Name = "Apellido")]
        public string? LastName { get; set; }

        [BsonElement("email")]
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [BsonElement("role")]
        public string? Role { get; set; }

        [Display(Name = "Nombre Completo")]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
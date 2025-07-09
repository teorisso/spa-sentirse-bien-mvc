// Models/Profesional.cs
using System.ComponentModel.DataAnnotations;

namespace SpaAdmin.Models
{
    public class Profesional
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string? NombreCompleto { get; set; }
        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public string? Especialidad { get; set; }
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string? Email { get; set; }
    }
}

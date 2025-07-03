// Models/Turno.cs
namespace SpaAdmin.Models
{
    public class Turno
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int ProfesionalId { get; set; }
        public int ServicioId { get; set; }
        public DateTime FechaHora { get; set; }

        // Relaciones (opcional si usas EF)
        public required Cliente Cliente { get; set; }
        public required Profesional Profesional { get; set; }
        public required Servicio Servicio { get; set; }
    }
}
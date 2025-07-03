// Models/Servicio.cs
namespace SpaAdmin.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Categoria { get; set; }
        public int DuracionMinutos { get; set; }
    }
}




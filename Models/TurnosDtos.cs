namespace SpaAdmin.Models
{
    public class UserDto
    {
        public string Id           { get; set; } = string.Empty;
        public string? FirstName   { get; set; }
        public string? LastName    { get; set; }
        public string? Email       { get; set; }
        public string? Role        { get; set; }
        public bool IsAdmin        { get; set; }
        public DateTime CreatedAt  { get; set; }
        
        // Propiedad calculada para mostrar nombre completo
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public class ServiceDto
    {
        public string Id        { get; set; } = string.Empty;
        public string Nombre    { get; set; } = string.Empty;
        public double Precio    { get; set; }
    }

    public class TurnoDto
    {
        public string Id           { get; set; } = string.Empty;
        public string ClienteId    { get; set; } = string.Empty;
        public string ServicioId   { get; set; } = string.Empty;
        public string ProfesionalId{ get; set; } = string.Empty;
        public DateTime Fecha      { get; set; }
        public string Hora         { get; set; } = string.Empty;
        public string Estado       { get; set; } = "pendiente";

        // Propiedades expandidas
        public UserDto?    Cliente      { get; set; }
        public ServiceDto? Servicio     { get; set; }
        public UserDto?    Profesional  { get; set; }
    }

    // Para leer la respuesta paginada de /api/turnos
    public class TurnosPaginated
    {
        public bool Success        { get; set; }
        public string Message      { get; set; } = string.Empty;
        public List<TurnoDto> Data { get; set; } = new();
        public int Page            { get; set; }
        public int TotalPages      { get; set; }
    }
}
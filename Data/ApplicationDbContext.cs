using Microsoft.EntityFrameworkCore;
using SpaAdmin.Models;

namespace SpaAdmin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

            public DbSet<Cliente> Clientes { get; set; }
            public DbSet<Profesional> Profesionales { get; set; }
            public DbSet<Servicio> Servicios { get; set; }
            public DbSet<Turno> Turnos { get; set; }
    }
}

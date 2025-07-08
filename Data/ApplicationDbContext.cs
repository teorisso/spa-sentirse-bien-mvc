using MongoDB.Driver;
using SpaAdmin.Models;

namespace SpaAdmin.Data
{
    public class ApplicationDbContext
    {
        private readonly IMongoDatabase _database;

        public ApplicationDbContext(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration.GetConnectionString("DatabaseName") ?? "sentirseBien";
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<Cliente> Clientes => _database.GetCollection<Cliente>("clientes");
        public IMongoCollection<Profesional> Profesionales => _database.GetCollection<Profesional>("profesionales");
        public IMongoCollection<Servicio> Servicios => _database.GetCollection<Servicio>("services");
        public IMongoCollection<Turno> Turnos => _database.GetCollection<Turno>("turnos");
    }
}

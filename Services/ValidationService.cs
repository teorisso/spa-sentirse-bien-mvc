using SpaAdmin.Data;
using SpaAdmin.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace SpaAdmin.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(ApplicationDbContext context, ILogger<ValidationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null)
        {
            try
            {
                var filter = Builders<Cliente>.Filter.Eq(c => c.Email, email);
                if (!string.IsNullOrEmpty(excludeId))
                {
                    filter &= Builders<Cliente>.Filter.Ne(c => c.Id, excludeId);
                }
                
                var exists = await _context.Clientes.Find(filter).AnyAsync();
                return !exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar email único: {Email}", email);
                return false;
            }
        }

        public async Task<bool> IsServiceNameUniqueAsync(string name, string? excludeId = null)
        {
            try
            {
                var filter = Builders<Servicio>.Filter.Eq(s => s.Nombre, name);
                if (!string.IsNullOrEmpty(excludeId))
                {
                    filter &= Builders<Servicio>.Filter.Ne(s => s.Id, excludeId);
                }
                
                var exists = await _context.Servicios.Find(filter).AnyAsync();
                return !exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar nombre de servicio único: {Name}", name);
                return false;
            }
        }

        public async Task<bool> IsProfessionalEmailUniqueAsync(string email, string? excludeId = null)
        {
            try
            {
                var filter = Builders<Profesional>.Filter.Eq("Email", email);
                if (!string.IsNullOrEmpty(excludeId))
                {
                    filter &= Builders<Profesional>.Filter.Ne("Id", excludeId);
                }
                
                var exists = await _context.Profesionales.Find(filter).AnyAsync();
                return !exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar email de profesional único: {Email}", email);
                return false;
            }
        }

        public async Task<bool> IsAppointmentTimeAvailableAsync(DateTime dateTime, string professionalId, string? excludeId = null)
        {
            try
            {
                // Validar que la fecha no sea en el pasado
                if (dateTime < DateTime.Now)
                {
                    return false;
                }

                // Validar que esté dentro del horario de trabajo (8:00 - 20:00)
                var hour = dateTime.Hour;
                if (hour < 8 || hour >= 20)
                {
                    return false;
                }

                // Buscar turnos del profesional en el rango de +/- 30 minutos
                var startTime = dateTime.AddMinutes(-30);
                var endTime = dateTime.AddMinutes(30);

                var filter = Builders<Turno>.Filter.Eq(t => t.ProfesionalId, professionalId);
                var turnos = await _context.Turnos.Find(filter).ToListAsync();

                foreach (var turno in turnos)
                {
                    // Combinar Fecha y Hora
                    if (DateTime.TryParse(turno.Hora, out var horaTurno))
                    {
                        var fechaHoraTurno = new DateTime(
                            turno.Fecha.Year, turno.Fecha.Month, turno.Fecha.Day,
                            horaTurno.Hour, horaTurno.Minute, 0
                        );
                        if (fechaHoraTurno >= startTime && fechaHoraTurno <= endTime)
                        {
                            if (excludeId == null || turno.Id != excludeId)
                                return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar disponibilidad de turno: {DateTime}, {ProfessionalId}", dateTime, professionalId);
                return false;
            }
        }

        public async Task<bool> IsClientExistsAsync(string clientId)
        {
            try
            {
                if (string.IsNullOrEmpty(clientId))
                    return false;
                
                var exists = await _context.Clientes.Find(c => c.Id == clientId).AnyAsync();
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar existencia de cliente: {ClientId}", clientId);
                return false;
            }
        }

        public async Task<bool> IsProfessionalExistsAsync(string professionalId)
        {
            try
            {
                if (string.IsNullOrEmpty(professionalId))
                    return false;
                
                if (!int.TryParse(professionalId, out int profIdInt))
                    return false;
                
                var filter = Builders<Profesional>.Filter.Eq("Id", profIdInt);
                var exists = await _context.Profesionales.Find(filter).AnyAsync();
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar existencia de profesional: {ProfessionalId}", professionalId);
                return false;
            }
        }

        public async Task<bool> IsServiceExistsAsync(string serviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceId))
                    return false;
                
                var exists = await _context.Servicios.Find(s => s.Id == serviceId).AnyAsync();
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar existencia de servicio: {ServiceId}", serviceId);
                return false;
            }
        }
    }
} 
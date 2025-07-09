using SpaAdmin.Models;

namespace SpaAdmin.Services
{
    public interface IValidationService
    {
        Task<bool> IsEmailUniqueAsync(string email, string? excludeId = null);
        Task<bool> IsServiceNameUniqueAsync(string name, string? excludeId = null);
        Task<bool> IsProfessionalEmailUniqueAsync(string email, string? excludeId = null);
        Task<bool> IsAppointmentTimeAvailableAsync(DateTime dateTime, string professionalId, string? excludeId = null);
        Task<bool> IsClientExistsAsync(string clientId);
        Task<bool> IsProfessionalExistsAsync(string professionalId);
        Task<bool> IsServiceExistsAsync(string serviceId);
    }
} 
using tp_final_backend.Models;

namespace tp_final_backend.Repositories.Interfaces
{
    public interface ITurnoRepository
    {
        Task<IEnumerable<Turno>> GetAllAsync();
        Task<Turno?> GetByIdAsync(int id);
        Task<IEnumerable<Turno>> GetByPacienteIdAsync(int pacienteId);
        Task<IEnumerable<Turno>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Turno>> GetByFechaAsync(DateTime fecha);
        Task<IEnumerable<Turno>> GetByEstadoAsync(string estado);
        Task<Turno> CreateAsync(Turno turno);
        Task<Turno> UpdateAsync(Turno turno);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> TurnoDisponibleAsync(int doctorId, DateTime fechaHora, int? turnoId = null);
        Task<IEnumerable<string>> GetSlotsDisponiblesAsync(int doctorId, DateTime fecha, int? turnoId = null);
    }
}

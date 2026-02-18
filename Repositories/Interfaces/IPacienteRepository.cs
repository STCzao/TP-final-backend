using tp_final_backend.Models;

namespace tp_final_backend.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<Paciente>> GetAllAsync();
        Task<Paciente?> GetByIdAsync(int id);
        Task<Paciente?> GetByDniAsync(string dni);
        Task<Paciente> CreateAsync(Paciente paciente);
        Task<Paciente> UpdateAsync(Paciente paciente);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByDniAsync(string dni);
    }
}

using tp_final_backend.Models;

namespace tp_final_backend.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<IEnumerable<Doctor>> GetActivosAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<Doctor?> GetByMatriculaAsync(string matricula);
        Task<IEnumerable<Doctor>> GetByEspecialidadAsync(string especialidad);
        Task<Doctor> CreateAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByMatriculaAsync(string matricula);
    }
}

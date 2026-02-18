using Microsoft.EntityFrameworkCore;
using tp_final_backend.Data;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Repositories
{
    public class DoctorRepository(ApplicationDbContext _context) : IDoctorRepository
    {

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctores
                .OrderBy(d => d.Apellido)
                .ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActivosAsync()
        {
            return await _context.Doctores
                .Where(d => d.Activo)
                .OrderBy(d => d.Apellido)
                .ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctores
                .Include(d => d.Turnos)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Doctor?> GetByMatriculaAsync(string matricula)
        {
            return await _context.Doctores
                .FirstOrDefaultAsync(d => d.Matricula == matricula);
        }

        public async Task<IEnumerable<Doctor>> GetByEspecialidadAsync(string especialidad)
        {
            return await _context.Doctores
                .Where(d => d.Especialidad.ToLower() == especialidad.ToLower())
                .OrderBy(d => d.Apellido)
                .ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<Doctor> CreateAsync(Doctor doctor)
        {
            _context.Doctores.Add(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<Doctor> UpdateAsync(Doctor doctor)
        {
            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _context.Doctores.FindAsync(id);
            if (doctor == null)
            {
                return false;
            }

            _context.Doctores.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Doctores.AnyAsync(d => d.Id == id);
        }

        public async Task<bool> ExistsByMatriculaAsync(string matricula)
        {
            return await _context.Doctores.AnyAsync(d => d.Matricula == matricula);
        }
    }
}

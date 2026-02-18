using Microsoft.EntityFrameworkCore;
using tp_final_backend.Data;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Repositories
{
    public class PacienteRepository(ApplicationDbContext context) : IPacienteRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            return await _context.Pacientes
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<Paciente?> GetByIdAsync(int id)
        {
            return await _context.Pacientes
                .Include(p => p.Turnos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Paciente?> GetByDniAsync(string dni)
        {
            return await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Dni == dni);
        }

        public async Task<Paciente> CreateAsync(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            await _context.SaveChangesAsync();
            return paciente;
        }

        public async Task<Paciente> UpdateAsync(Paciente paciente)
        {
            _context.Entry(paciente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return paciente;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var paciente = await _context.Pacientes.FindAsync(id);
            if (paciente == null)
            {
                return false;
            }

            _context.Pacientes.Remove(paciente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Pacientes.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByDniAsync(string dni)
        {
            return await _context.Pacientes.AnyAsync(p => p.Dni == dni);
        }
    }
}

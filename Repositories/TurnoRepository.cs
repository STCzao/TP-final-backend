using Microsoft.EntityFrameworkCore;
using tp_final_backend.Data;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly ApplicationDbContext _context;

        public TurnoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Turno>> GetAllAsync()
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
        }

        public async Task<Turno?> GetByIdAsync(int id)
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Turno>> GetByPacienteIdAsync(int pacienteId)
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .Where(t => t.PacienteId == pacienteId)
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .Where(t => t.DoctorId == doctorId)
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByFechaAsync(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fechaInicio.AddDays(1);

            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .Where(t => t.FechaHora >= fechaInicio && t.FechaHora < fechaFin)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turno>> GetByEstadoAsync(string estado)
        {
            return await _context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Doctor)
                .Where(t => t.Estado.ToLower() == estado.ToLower())
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
        }

        public async Task<Turno> CreateAsync(Turno turno)
        {
            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();
            
            // Recargar con las relaciones
            await _context.Entry(turno)
                .Reference(t => t.Paciente)
                .LoadAsync();
            await _context.Entry(turno)
                .Reference(t => t.Doctor)
                .LoadAsync();
            
            return turno;
        }

        public async Task<Turno> UpdateAsync(Turno turno)
        {
            _context.Entry(turno).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            // Recargar con las relaciones
            await _context.Entry(turno)
                .Reference(t => t.Paciente)
                .LoadAsync();
            await _context.Entry(turno)
                .Reference(t => t.Doctor)
                .LoadAsync();
            
            return turno;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null)
            {
                return false;
            }

            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Turnos.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TurnoDisponibleAsync(int doctorId, DateTime fechaHora, int? turnoId = null)
        {
            // Verificar si el doctor ya tiene un turno en ese horario
            // Consideramos un margen de 30 minutos antes y después
            var fechaInicio = fechaHora.AddMinutes(-30);
            var fechaFin = fechaHora.AddMinutes(30);

            var query = _context.Turnos
                .Where(t => t.DoctorId == doctorId &&
                           t.FechaHora >= fechaInicio &&
                           t.FechaHora <= fechaFin &&
                           t.Estado != "Cancelado");

            // Si estamos actualizando un turno, excluirlo de la verificación
            if (turnoId.HasValue)
            {
                query = query.Where(t => t.Id != turnoId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}

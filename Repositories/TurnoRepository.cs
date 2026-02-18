using Microsoft.EntityFrameworkCore;
using tp_final_backend.Data;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Repositories
{
    public class TurnoRepository(ApplicationDbContext _context) : ITurnoRepository
    {

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
                .Where(t => string.Equals(t.Estado, estado, StringComparison.OrdinalIgnoreCase))
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
            // Normalizar a UTC
            if (fechaHora.Kind == DateTimeKind.Unspecified)
                fechaHora = DateTime.SpecifyKind(fechaHora, DateTimeKind.Utc);
            else if (fechaHora.Kind == DateTimeKind.Local)
                fechaHora = fechaHora.ToUniversalTime();

            // Truncar segundos
            fechaHora = new DateTime(fechaHora.Year, fechaHora.Month, fechaHora.Day,
                fechaHora.Hour, fechaHora.Minute, 0, DateTimeKind.Utc);

            var duracion = TimeSpan.FromMinutes(30);
            var inicioNuevo = fechaHora;
            var finNuevo = fechaHora.Add(duracion);

            // Verificar solapamiento: un turno existente solapa si su inicio < finNuevo Y su fin > inicioNuevo
            var existe = await _context.Turnos
                .Where(t => t.DoctorId == doctorId
                    && (turnoId == null || t.Id != turnoId)
                    && t.FechaHora < finNuevo
                    && t.FechaHora.AddMinutes(30) > inicioNuevo)
                .AnyAsync();

            return !existe;
        }

        public async Task<IEnumerable<string>> GetSlotsDisponiblesAsync(int doctorId, DateTime fecha, int? turnoId = null)
        {
            var fechaUtc = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);

            // Traer todos los turnos del doctor en esa fecha
            var turnosDelDia = await _context.Turnos
                .Where(t => t.DoctorId == doctorId
                    && t.FechaHora >= fechaUtc
                    && t.FechaHora < fechaUtc.AddDays(1)
                    && (turnoId == null || t.Id != turnoId))
                .Select(t => t.FechaHora)
                .ToListAsync();

            var slots = new List<string>();
            var slotInicio = new TimeSpan(9, 0, 0);
            var slotFin = new TimeSpan(18, 0, 0);
            var duracion = TimeSpan.FromMinutes(30);

            for (var hora = slotInicio; hora < slotFin; hora += duracion)
            {
                var slotDt = fechaUtc.Add(hora);
                // El slot está ocupado si algún turno existente solapa con él
                var ocupado = turnosDelDia.Any(t => t < slotDt.Add(duracion) && t.Add(duracion) > slotDt);
                if (!ocupado)
                    slots.Add(hora.ToString(@"hh\:mm"));
            }

            return slots;
        }
    }
}

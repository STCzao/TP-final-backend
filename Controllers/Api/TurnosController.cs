using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnosController(
        ITurnoRepository turnoRepository,
        IPacienteRepository pacienteRepository,
        IDoctorRepository doctorRepository,
        IMapper mapper) : ControllerBase
    {

        // GET: api/Turnos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnos()
        {
            var turnos = await turnoRepository.GetAllAsync();
            var turnosDto = mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return Ok(turnosDto);
        }

        // GET: api/Turnos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TurnoDto>> GetTurno(int id)
        {
            var turno = await turnoRepository.GetByIdAsync(id);

            if (turno == null)
            {
                return NotFound(new { message = $"Turno con ID {id} no encontrado" });
            }

            var turnoDto = mapper.Map<TurnoDto>(turno);
            return Ok(turnoDto);
        }

        // GET: api/Turnos/paciente/5
        [HttpGet("paciente/{pacienteId}")]
        public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnosByPaciente(int pacienteId)
        {
            if (!await pacienteRepository.ExistsAsync(pacienteId))
            {
                return NotFound(new { message = $"Paciente con ID {pacienteId} no encontrado" });
            }

            var turnos = await turnoRepository.GetByPacienteIdAsync(pacienteId);
            var turnosDto = mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return Ok(turnosDto);
        }

        // GET: api/Turnos/doctor/5
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnosByDoctor(int doctorId)
        {
            if (!await doctorRepository.ExistsAsync(doctorId))
            {
                return NotFound(new { message = $"Doctor con ID {doctorId} no encontrado" });
            }

            var turnos = await turnoRepository.GetByDoctorIdAsync(doctorId);
            var turnosDto = mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return Ok(turnosDto);
        }

        // GET: api/Turnos/fecha/2024-12-01
        [HttpGet("fecha/{fecha}")]
        public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnosByFecha(DateTime fecha)
        {
            var turnos = await turnoRepository.GetByFechaAsync(fecha);
            var turnosDto = mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return Ok(turnosDto);
        }

        // GET: api/Turnos/slots?doctorId=1&fecha=2026-02-19
        // GET: api/Turnos/slots?doctorId=1&fecha=2026-02-19&turnoId=5  (excluye el turno al editar)
        [HttpGet("slots")]
        public async Task<ActionResult<IEnumerable<string>>> GetSlotsDisponibles(
            [FromQuery] int doctorId,
            [FromQuery] DateTime fecha,
            [FromQuery] int? turnoId = null)
        {
            if (!await doctorRepository.ExistsAsync(doctorId))
                return NotFound(new { message = $"Doctor con ID {doctorId} no encontrado" });

            var slots = await turnoRepository.GetSlotsDisponiblesAsync(doctorId, fecha, turnoId);
            return Ok(slots);
        }

        // GET: api/Turnos/estado/Pendiente
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<TurnoDto>>> GetTurnosByEstado(string estado)
        {
            var turnos = await turnoRepository.GetByEstadoAsync(estado);
            var turnosDto = mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return Ok(turnosDto);
        }

        // POST: api/Turnos
        [HttpPost]
        public async Task<ActionResult<TurnoDto>> CreateTurno(CreateTurnoDto createTurnoDto)
        {
            // Validar que el paciente existe
            if (!await pacienteRepository.ExistsAsync(createTurnoDto.PacienteId))
            {
                return BadRequest(new { message = $"Paciente con ID {createTurnoDto.PacienteId} no encontrado" });
            }

            // Validar que el doctor existe
            if (!await doctorRepository.ExistsAsync(createTurnoDto.DoctorId))
            {
                return BadRequest(new { message = $"Doctor con ID {createTurnoDto.DoctorId} no encontrado" });
            }

            // Validar que la fecha no sea en el pasado
            if (createTurnoDto.FechaHora < DateTime.Now)
            {
                return BadRequest(new { message = "No se puede crear un turno con fecha en el pasado" });
            }

            // Validar disponibilidad del doctor
            if (!await turnoRepository.TurnoDisponibleAsync(createTurnoDto.DoctorId, createTurnoDto.FechaHora))
            {
                return BadRequest(new { message = "El doctor ya tiene un turno en ese horario" });
            }

            var turno = mapper.Map<Turno>(createTurnoDto);
            var createdTurno = await turnoRepository.CreateAsync(turno);
            var turnoDto = mapper.Map<TurnoDto>(createdTurno);

            return CreatedAtAction(nameof(GetTurno), new { id = turnoDto.Id }, turnoDto);
        }

        // PUT: api/Turnos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TurnoDto>> UpdateTurno(int id, UpdateTurnoDto updateTurnoDto)
        {
            var existingTurno = await turnoRepository.GetByIdAsync(id);
            if (existingTurno == null)
            {
                return NotFound(new { message = $"Turno con ID {id} no encontrado" });
            }

            // Validar que el paciente existe
            if (!await pacienteRepository.ExistsAsync(updateTurnoDto.PacienteId))
            {
                return BadRequest(new { message = $"Paciente con ID {updateTurnoDto.PacienteId} no encontrado" });
            }

            // Validar que el doctor existe
            if (!await doctorRepository.ExistsAsync(updateTurnoDto.DoctorId))
            {
                return BadRequest(new { message = $"Doctor con ID {updateTurnoDto.DoctorId} no encontrado" });
            }

            // Validar disponibilidad del doctor (excluyendo el turno actual)
            if (!await turnoRepository.TurnoDisponibleAsync(updateTurnoDto.DoctorId, updateTurnoDto.FechaHora, id))
            {
                return BadRequest(new { message = "El doctor ya tiene un turno en ese horario" });
            }

            mapper.Map(updateTurnoDto, existingTurno);
            var updatedTurno = await turnoRepository.UpdateAsync(existingTurno);
            var turnoDto = mapper.Map<TurnoDto>(updatedTurno);

            return Ok(turnoDto);
        }

        // PATCH: api/Turnos/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<ActionResult<TurnoDto>> UpdateEstadoTurno(int id, [FromBody] string estado)
        {
            var turno = await turnoRepository.GetByIdAsync(id);
            if (turno == null)
            {
                return NotFound(new { message = $"Turno con ID {id} no encontrado" });
            }

            var estadosValidos = new[] { "Pendiente", "Confirmado", "Cancelado", "Completado" };
            if (!estadosValidos.Contains(estado))
            {
                return BadRequest(new { message = $"Estado inválido. Estados válidos: {string.Join(", ", estadosValidos)}" });
            }

            turno.Estado = estado;
            var updatedTurno = await turnoRepository.UpdateAsync(turno);
            var turnoDto = mapper.Map<TurnoDto>(updatedTurno);

            return Ok(turnoDto);
        }

        // DELETE: api/Turnos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTurno(int id)
        {
            if (!await turnoRepository.ExistsAsync(id))
            {
                return NotFound(new { message = $"Turno con ID {id} no encontrado" });
            }

            var deleted = await turnoRepository.DeleteAsync(id);
            if (!deleted)
            {
                return BadRequest(new { message = "No se pudo eliminar el turno" });
            }

            return NoContent();
        }
    }
}

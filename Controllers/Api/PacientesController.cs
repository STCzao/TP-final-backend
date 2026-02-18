using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteRepository _repository;
        private readonly IMapper _mapper;

        public PacientesController(IPacienteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Pacientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteDto>>> GetPacientes()
        {
            var pacientes = await _repository.GetAllAsync();
            var pacientesDto = _mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return Ok(pacientesDto);
        }

        // GET: api/Pacientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteDto>> GetPaciente(int id)
        {
            var paciente = await _repository.GetByIdAsync(id);

            if (paciente == null)
            {
                return NotFound(new { message = $"Paciente con ID {id} no encontrado" });
            }

            var pacienteDto = _mapper.Map<PacienteDto>(paciente);
            return Ok(pacienteDto);
        }

        // GET: api/Pacientes/dni/12345678
        [HttpGet("dni/{dni}")]
        public async Task<ActionResult<PacienteDto>> GetPacienteByDni(string dni)
        {
            var paciente = await _repository.GetByDniAsync(dni);

            if (paciente == null)
            {
                return NotFound(new { message = $"Paciente con DNI {dni} no encontrado" });
            }

            var pacienteDto = _mapper.Map<PacienteDto>(paciente);
            return Ok(pacienteDto);
        }

        // POST: api/Pacientes
        [HttpPost]
        public async Task<ActionResult<PacienteDto>> CreatePaciente(CreatePacienteDto createPacienteDto)
        {
            // Validar que no exista un paciente con el mismo DNI
            if (await _repository.ExistsByDniAsync(createPacienteDto.Dni))
            {
                return BadRequest(new { message = $"Ya existe un paciente con DNI {createPacienteDto.Dni}" });
            }

            var paciente = _mapper.Map<Paciente>(createPacienteDto);
            var createdPaciente = await _repository.CreateAsync(paciente);
            var pacienteDto = _mapper.Map<PacienteDto>(createdPaciente);

            return CreatedAtAction(nameof(GetPaciente), new { id = pacienteDto.Id }, pacienteDto);
        }

        // PUT: api/Pacientes/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PacienteDto>> UpdatePaciente(int id, UpdatePacienteDto updatePacienteDto)
        {
            var existingPaciente = await _repository.GetByIdAsync(id);
            if (existingPaciente == null)
            {
                return NotFound(new { message = $"Paciente con ID {id} no encontrado" });
            }

            // Validar que el DNI no esté en uso por otro paciente
            var pacienteConMismoDni = await _repository.GetByDniAsync(updatePacienteDto.Dni);
            if (pacienteConMismoDni != null && pacienteConMismoDni.Id != id)
            {
                return BadRequest(new { message = $"El DNI {updatePacienteDto.Dni} ya está en uso por otro paciente" });
            }

            _mapper.Map(updatePacienteDto, existingPaciente);
            var updatedPaciente = await _repository.UpdateAsync(existingPaciente);
            var pacienteDto = _mapper.Map<PacienteDto>(updatedPaciente);

            return Ok(pacienteDto);
        }

        // DELETE: api/Pacientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaciente(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                return NotFound(new { message = $"Paciente con ID {id} no encontrado" });
            }

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
            {
                return BadRequest(new { message = "No se pudo eliminar el paciente" });
            }

            return NoContent();
        }
    }
}

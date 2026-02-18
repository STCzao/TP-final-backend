using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctoresController(IDoctorRepository repository, IMapper mapper) : ControllerBase
    {
        private readonly IDoctorRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        // GET: api/Doctores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctores()
        {
            var doctores = await _repository.GetAllAsync();
            var doctoresDto = _mapper.Map<IEnumerable<DoctorDto>>(doctores);
            return Ok(doctoresDto);
        }

        // GET: api/Doctores/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctoresActivos()
        {
            var doctores = await _repository.GetActivosAsync();
            var doctoresDto = _mapper.Map<IEnumerable<DoctorDto>>(doctores);
            return Ok(doctoresDto);
        }

        // GET: api/Doctores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);

            if (doctor == null)
            {
                return NotFound(new { message = $"Doctor con ID {id} no encontrado" });
            }

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        // GET: api/Doctores/matricula/MP12345
        [HttpGet("matricula/{matricula}")]
        public async Task<ActionResult<DoctorDto>> GetDoctorByMatricula(string matricula)
        {
            var doctor = await _repository.GetByMatriculaAsync(matricula);

            if (doctor == null)
            {
                return NotFound(new { message = $"Doctor con matrícula {matricula} no encontrado" });
            }

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        // GET: api/Doctores/especialidad/Cardiologia
        [HttpGet("especialidad/{especialidad}")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctoresByEspecialidad(string especialidad)
        {
            var doctores = await _repository.GetByEspecialidadAsync(especialidad);
            var doctoresDto = _mapper.Map<IEnumerable<DoctorDto>>(doctores);
            return Ok(doctoresDto);
        }

        // POST: api/Doctores
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> CreateDoctor(CreateDoctorDto createDoctorDto)
        {
            // Validar que no exista un doctor con la misma matrícula
            if (await _repository.ExistsByMatriculaAsync(createDoctorDto.Matricula))
            {
                return BadRequest(new { message = $"Ya existe un doctor con matrícula {createDoctorDto.Matricula}" });
            }

            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            var createdDoctor = await _repository.CreateAsync(doctor);
            var doctorDto = _mapper.Map<DoctorDto>(createdDoctor);

            return CreatedAtAction(nameof(GetDoctor), new { id = doctorDto.Id }, doctorDto);
        }

        // PUT: api/Doctores/5
        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDto>> UpdateDoctor(int id, UpdateDoctorDto updateDoctorDto)
        {
            var existingDoctor = await _repository.GetByIdAsync(id);
            if (existingDoctor == null)
            {
                return NotFound(new { message = $"Doctor con ID {id} no encontrado" });
            }

            // Validar que la matrícula no esté en uso por otro doctor
            var doctorConMismaMatricula = await _repository.GetByMatriculaAsync(updateDoctorDto.Matricula);
            if (doctorConMismaMatricula != null && doctorConMismaMatricula.Id != id)
            {
                return BadRequest(new { message = $"La matrícula {updateDoctorDto.Matricula} ya está en uso por otro doctor" });
            }

            _mapper.Map(updateDoctorDto, existingDoctor);
            var updatedDoctor = await _repository.UpdateAsync(existingDoctor);
            var doctorDto = _mapper.Map<DoctorDto>(updatedDoctor);

            return Ok(doctorDto);
        }

        // DELETE: api/Doctores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            if (!await _repository.ExistsAsync(id))
            {
                return NotFound(new { message = $"Doctor con ID {id} no encontrado" });
            }

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
            {
                return BadRequest(new { message = "No se pudo eliminar el doctor" });
            }

            return NoContent();
        }
    }
}

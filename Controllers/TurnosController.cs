using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers
{
    public class TurnosController : Controller
    {
        private readonly ITurnoRepository _turnoRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public TurnosController(
            ITurnoRepository turnoRepository,
            IPacienteRepository pacienteRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _turnoRepository = turnoRepository;
            _pacienteRepository = pacienteRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var turnos = await _turnoRepository.GetAllAsync();
            var turnosDto = _mapper.Map<IEnumerable<TurnoDto>>(turnos);
            return View(turnosDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var turno = await _turnoRepository.GetByIdAsync(id);
            if (turno == null) return NotFound();
            var turnoDto = _mapper.Map<TurnoDto>(turno);
            return View(turnoDto);
        }

        public async Task<IActionResult> Create()
        {
            await LoadSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTurnoDto createTurnoDto)
        {
            if (ModelState.IsValid)
            {
                if (!await _pacienteRepository.ExistsAsync(createTurnoDto.PacienteId))
                {
                    ModelState.AddModelError("PacienteId", "Paciente no encontrado");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                if (!await _doctorRepository.ExistsAsync(createTurnoDto.DoctorId))
                {
                    ModelState.AddModelError("DoctorId", "Doctor no encontrado");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                if (createTurnoDto.FechaHora < DateTime.Now)
                {
                    ModelState.AddModelError("FechaHora", "No se puede crear un turno con fecha en el pasado");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                if (!await _turnoRepository.TurnoDisponibleAsync(createTurnoDto.DoctorId, createTurnoDto.FechaHora))
                {
                    ModelState.AddModelError("FechaHora", "El doctor ya tiene un turno en ese horario");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                var turno = _mapper.Map<Turno>(createTurnoDto);
                await _turnoRepository.CreateAsync(turno);
                TempData["Success"] = "Turno creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            await LoadSelectLists();
            return View(createTurnoDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var turno = await _turnoRepository.GetByIdAsync(id);
            if (turno == null) return NotFound();
            var updateDto = _mapper.Map<UpdateTurnoDto>(turno);
            ViewBag.Id = id;
            await LoadSelectLists();
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTurnoDto updateTurnoDto)
        {
            if (ModelState.IsValid)
            {
                var existingTurno = await _turnoRepository.GetByIdAsync(id);
                if (existingTurno == null) return NotFound();

                if (!await _pacienteRepository.ExistsAsync(updateTurnoDto.PacienteId))
                {
                    ModelState.AddModelError("PacienteId", "Paciente no encontrado");
                    ViewBag.Id = id;
                    await LoadSelectLists();
                    return View(updateTurnoDto);
                }

                if (!await _doctorRepository.ExistsAsync(updateTurnoDto.DoctorId))
                {
                    ModelState.AddModelError("DoctorId", "Doctor no encontrado");
                    ViewBag.Id = id;
                    await LoadSelectLists();
                    return View(updateTurnoDto);
                }

                if (!await _turnoRepository.TurnoDisponibleAsync(updateTurnoDto.DoctorId, updateTurnoDto.FechaHora, id))
                {
                    ModelState.AddModelError("FechaHora", "El doctor ya tiene un turno en ese horario");
                    ViewBag.Id = id;
                    await LoadSelectLists();
                    return View(updateTurnoDto);
                }

                _mapper.Map(updateTurnoDto, existingTurno);
                await _turnoRepository.UpdateAsync(existingTurno);
                TempData["Success"] = "Turno actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Id = id;
            await LoadSelectLists();
            return View(updateTurnoDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var turno = await _turnoRepository.GetByIdAsync(id);
            if (turno == null) return NotFound();
            var turnoDto = _mapper.Map<TurnoDto>(turno);
            return View(turnoDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _turnoRepository.DeleteAsync(id);
            TempData["Success"] = "Turno eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadSelectLists()
        {
            var pacientes = await _pacienteRepository.GetAllAsync();
            var doctores = await _doctorRepository.GetActivosAsync();

            ViewBag.Pacientes = new SelectList(
                pacientes.Select(p => new { 
                    Id = p.Id, 
                    NombreCompleto = $"{p.Apellido}, {p.Nombre} (DNI: {p.Dni})"
                }), 
                "Id", 
                "NombreCompleto"
            );

            ViewBag.Doctores = new SelectList(
                doctores.Select(d => new { 
                    Id = d.Id, 
                    NombreCompleto = $"Dr. {d.Apellido}, {d.Nombre} - {d.Especialidad}"
                }), 
                "Id", 
                "NombreCompleto"
            );

            ViewBag.Estados = new SelectList(new[] 
            { 
                "Pendiente", 
                "Confirmado", 
                "Cancelado", 
                "Completado" 
            });
        }
    }
}

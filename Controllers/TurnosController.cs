using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers
{
    public class TurnosController(
        ITurnoRepository turnoRepository,
        IPacienteRepository pacienteRepository,
        IDoctorRepository doctorRepository,
        IMapper mapper) : Controller
    {
        private readonly ITurnoRepository _turnoRepository = turnoRepository;
        private readonly IPacienteRepository _pacienteRepository = pacienteRepository;
        private readonly IDoctorRepository _doctorRepository = doctorRepository;
        private readonly IMapper _mapper = mapper;

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
                var turno = _mapper.Map<Turno>(createTurnoDto);
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

                var manana = DateTime.Today.AddDays(1);
                if (createTurnoDto.FechaHora.Date < manana.Date)
                {
                    ModelState.AddModelError("FechaHora", "El turno debe agendarse a partir del día de mañana");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                if (createTurnoDto.FechaHora.Hour < 9 || createTurnoDto.FechaHora.Hour >= 18)
                {
                    ModelState.AddModelError("FechaHora", "El horario debe estar entre las 9:00 y las 18:00");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                if (!await _turnoRepository.TurnoDisponibleAsync(createTurnoDto.DoctorId, createTurnoDto.FechaHora))
                {
                    ModelState.AddModelError("FechaHora", "El doctor ya tiene un turno en ese horario");
                    await LoadSelectLists();
                    return View(createTurnoDto);
                }

                
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

                var manana = DateTime.Today.AddDays(1);
                if (updateTurnoDto.FechaHora.Date < manana.Date)
                {
                    ModelState.AddModelError("FechaHora", "El turno debe agendarse a partir del día de mañana");
                    ViewBag.Id = id;
                    await LoadSelectLists();
                    return View(updateTurnoDto);
                }

                if (updateTurnoDto.FechaHora.Hour < 9 || updateTurnoDto.FechaHora.Hour >= 18)
                {
                    ModelState.AddModelError("FechaHora", "El horario debe estar entre las 9:00 y las 18:00");
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

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers
{
    public class PacientesController : Controller
    {
        private readonly IPacienteRepository _repository;
        private readonly IMapper _mapper;

        public PacientesController(IPacienteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            var pacientes = await _repository.GetAllAsync();
            var pacientesDto = _mapper.Map<IEnumerable<PacienteDto>>(pacientes);
            return View(pacientesDto);
        }

        // GET: Pacientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            var pacienteDto = _mapper.Map<PacienteDto>(paciente);
            return View(pacienteDto);
        }

        // GET: Pacientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePacienteDto createPacienteDto)
        {
            if (ModelState.IsValid)
            {
                if (await _repository.ExistsByDniAsync(createPacienteDto.Dni))
                {
                    ModelState.AddModelError("Dni", $"Ya existe un paciente con DNI {createPacienteDto.Dni}");
                    return View(createPacienteDto);
                }

                var paciente = _mapper.Map<Paciente>(createPacienteDto);
                await _repository.CreateAsync(paciente);
                TempData["Success"] = "Paciente creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(createPacienteDto);
        }

        // GET: Pacientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            var updateDto = _mapper.Map<UpdatePacienteDto>(paciente);
            ViewBag.Id = id;
            return View(updateDto);
        }

        // POST: Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePacienteDto updatePacienteDto)
        {
            if (ModelState.IsValid)
            {
                var existingPaciente = await _repository.GetByIdAsync(id);
                if (existingPaciente == null)
                {
                    return NotFound();
                }

                var pacienteConMismoDni = await _repository.GetByDniAsync(updatePacienteDto.Dni);
                if (pacienteConMismoDni != null && pacienteConMismoDni.Id != id)
                {
                    ModelState.AddModelError("Dni", $"El DNI {updatePacienteDto.Dni} ya está en uso por otro paciente");
                    ViewBag.Id = id;
                    return View(updatePacienteDto);
                }

                _mapper.Map(updatePacienteDto, existingPaciente);
                await _repository.UpdateAsync(existingPaciente);
                TempData["Success"] = "Paciente actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Id = id;
            return View(updatePacienteDto);
        }

        // GET: Pacientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            var pacienteDto = _mapper.Map<PacienteDto>(paciente);
            return View(pacienteDto);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            TempData["Success"] = "Paciente eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}

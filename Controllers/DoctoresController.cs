using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using tp_final_backend.DTOs;
using tp_final_backend.Models;
using tp_final_backend.Repositories.Interfaces;

namespace tp_final_backend.Controllers
{
    public class DoctoresController : Controller
    {
        private readonly IDoctorRepository _repository;
        private readonly IMapper _mapper;

        public DoctoresController(IDoctorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var doctores = await _repository.GetAllAsync();
            var doctoresDto = _mapper.Map<IEnumerable<DoctorDto>>(doctores);
            return View(doctoresDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return View(doctorDto);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDoctorDto createDoctorDto)
        {
            if (ModelState.IsValid)
            {
                if (await _repository.ExistsByMatriculaAsync(createDoctorDto.Matricula))
                {
                    ModelState.AddModelError("Matricula", $"Ya existe un doctor con matrícula {createDoctorDto.Matricula}");
                    return View(createDoctorDto);
                }

                var doctor = _mapper.Map<Doctor>(createDoctorDto);
                await _repository.CreateAsync(doctor);
                TempData["Success"] = "Doctor creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(createDoctorDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            var updateDto = _mapper.Map<UpdateDoctorDto>(doctor);
            ViewBag.Id = id;
            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDoctorDto updateDoctorDto)
        {
            if (ModelState.IsValid)
            {
                var existingDoctor = await _repository.GetByIdAsync(id);
                if (existingDoctor == null) return NotFound();

                var doctorConMismaMatricula = await _repository.GetByMatriculaAsync(updateDoctorDto.Matricula);
                if (doctorConMismaMatricula != null && doctorConMismaMatricula.Id != id)
                {
                    ModelState.AddModelError("Matricula", $"La matrícula {updateDoctorDto.Matricula} ya está en uso");
                    ViewBag.Id = id;
                    return View(updateDoctorDto);
                }

                _mapper.Map(updateDoctorDto, existingDoctor);
                await _repository.UpdateAsync(existingDoctor);
                TempData["Success"] = "Doctor actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Id = id;
            return View(updateDoctorDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _repository.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return View(doctorDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.DeleteAsync(id);
            TempData["Success"] = "Doctor eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tadawi.Models;
using Tadawi.Repositories.PatientFileRepository;

namespace Tadawi.Controllers
{
    public class PatientFilesController : Controller
    {
        private readonly IPatientFileRepository _patientFileRepository;

        public PatientFilesController(IPatientFileRepository patientFileRepository)
        {
            _patientFileRepository = patientFileRepository;
        }

        // GET: PatientFiles
        public async Task<IActionResult> Index()
        {
            var files = await _patientFileRepository.GetAsync();
            return View(files);
        }

        // GET: PatientFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var patientFile = await _patientFileRepository.GetByIdAsync(id.Value);
            if (patientFile == null) return NotFound();

            return View(patientFile);
        }

        // GET: PatientFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PatientFiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Diagnosis")] PatientFile patientFile)
        {
            if (ModelState.IsValid)
            {
                int id = await _patientFileRepository.AddAsync(patientFile.Diagnosis);
                if (id > 0)
                    return RedirectToAction(nameof(Index));
            }

            return View(patientFile);
        }

        // GET: PatientFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var patientFile = await _patientFileRepository.GetByIdAsync(id.Value);
            if (patientFile == null) return NotFound();

            return View(patientFile);
        }

        // POST: PatientFiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Diagnosis")] PatientFile patientFile)
        {
            if (id != patientFile.Id) return NotFound();

            if (ModelState.IsValid)
            {
                bool updated = await _patientFileRepository.UpdateAsync(patientFile);
                if (updated)
                    return RedirectToAction(nameof(Index));
            }

            return View(patientFile);
        }

        // GET: PatientFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patientFile = await _patientFileRepository.GetByIdAsync(id.Value);
            if (patientFile == null) return NotFound();

            return View(patientFile);
        }

        // POST: PatientFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _patientFileRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

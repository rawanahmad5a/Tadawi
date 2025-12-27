using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.VisitRepository;

namespace Tadawi.Controllers
{
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IVisitRepository _visitRepository;

        public VisitsController(ApplicationDbContext context, IVisitRepository visitRepository)
        {
            _context = context;
            _visitRepository = visitRepository;
        }

        // GET: Visits
        public async Task<IActionResult> Index()
        {
            var data = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType);

            return View(await data.ToListAsync());
        }

        // GET: Visits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var visit = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visit == null)
                return NotFound();

            return View(visit);
        }

        // GET: Visits/Create
        public IActionResult Create()
        {
            var doctors = _context.Doctors.ToList();
            ViewBag.Specializations = doctors.Select(d => d.Specialization).Distinct().ToList();
            ViewBag.DoctorsJson = doctors.Select(d => new { d.Id, d.Name, d.Specialization }).ToList();

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name");
            ViewData["DoctorId"] = new SelectList(doctors, "Id", "Name");
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Description");
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "DiseaseName");
            return View();
        }

        // POST: Visits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,VisitDate,VisitType,PatientId,DoctorId")]
            Visit visit)
        {
            // Remove validation for fields we removed from the UI
            ModelState.Remove("DiagnosisId");
            ModelState.Remove("DiseaseTypeId");
            ModelState.Remove("Diagnosis");
            ModelState.Remove("DiseaseType");
            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");

            // Fallback for foreign keys to prevent FK constraint errors in DB
            var defaultDiagnosis = await _context.Diagnoses.FirstOrDefaultAsync();
            if (defaultDiagnosis != null && visit.DiagnosisId == 0) visit.DiagnosisId = defaultDiagnosis.Id;

            var defaultDisease = await _context.DiseaseTypes.FirstOrDefaultAsync();
            if (defaultDisease != null && visit.DiseaseTypeId == 0) visit.DiseaseTypeId = defaultDisease.Id;

            if (ModelState.IsValid)
            {
                if (await _visitRepository.AddAsync(visit))
                {
                    return RedirectToAction("Index", "Patients");
                }
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", visit.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", visit.DoctorId);
            // ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Description", visit.DiagnosisId);
            // ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "DiseaseName", visit.DiseaseTypeId);

            return View(visit);
        }

        // GET: Visits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var visit = await _context.Visits.FindAsync(id);
            if (visit == null)
                return NotFound();

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", visit.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", visit.DoctorId);
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Description", visit.DiagnosisId);
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "DiseaseName", visit.DiseaseTypeId);

            return View(visit);
        }

        // POST: Visits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,VisitDate,VisitType,PatientId,DoctorId,DiagnosisId,DiseaseTypeId")]
            Visit visit)
        {
            if (id != visit.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Visits.Any(e => e.Id == visit.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", visit.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Name", visit.DoctorId);
            ViewData["DiagnosisId"] = new SelectList(_context.Diagnoses, "Id", "Description", visit.DiagnosisId);
            ViewData["DiseaseTypeId"] = new SelectList(_context.DiseaseTypes, "Id", "DiseaseName", visit.DiseaseTypeId);

            return View(visit);
        }

        // GET: Visits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var visit = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visit == null)
                return NotFound();

            return View(visit);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.PatientRepository;

namespace Tadawi.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPatientRepository _patientRepository;
        public PatientsController(ApplicationDbContext context, IPatientRepository patientRepository)
        {
            _context = context;
            _patientRepository = patientRepository;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Patients.Include(p => p.PatientFile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                 .Include(p => p.PatientFile)
                 .ThenInclude(pf => pf.Visits)
                 .ThenInclude(v => v.Doctor)
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            var visits = await _context.Visits
             .Where(v => v.PatientId == id)
              .Include(v => v.Doctor)
              .OrderByDescending(v => v.VisitDate)
              .ToListAsync();
            ViewBag.PatientVisits = visits;
            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            var doctors = _context.Doctors.ToList();
            ViewBag.Specializations = doctors.Select(d => d.Specialization).Distinct().ToList();
            ViewBag.DoctorsJson = doctors.Select(d => new { d.Id, d.Name, d.Specialization }).ToList();

            ViewBag.DoctorId = new SelectList(doctors, "Id", "Name");
            ViewData["PatientFileId"] = new SelectList(_context.PatientFiles, "Id", "Id");
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Phone,Age,Gender,Address")] Patient patient)
        {
            // STEP 1: Create PatientFile first
            var patientFile = new PatientFile
            {
                Diagnosis = "ملف جديد - لم يتم التشخيص بعد"
            };
            _context.PatientFiles.Add(patientFile);
            await _context.SaveChangesAsync();

            // STEP 2: Assign PatientFileId to patient
            patient.PatientFileId = patientFile.Id;

            // STEP 3: Remove validation for navigation properties
            ModelState.Remove("PatientFile");
            ModelState.Remove("Visits");
            ModelState.Remove("Diagnosis");

            // DEBUG LOGGING
            Console.WriteLine("====== PATIENT CREATE DEBUG ======");
            Console.WriteLine($"Patient Name: {patient.Name}");
            Console.WriteLine($"Patient Phone: {patient.Phone}");
            Console.WriteLine($"Patient Age: {patient.Age}");
            Console.WriteLine($"Patient Gender: {patient.Gender}");
            Console.WriteLine($"Patient Address: {patient.Address}");
            Console.WriteLine($"PatientFileId Assigned: {patient.PatientFileId}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine(">>> ModelState Contains Errors <<<");
                foreach (var modelState in ModelState)
                {
                    Console.WriteLine($"  Field: {modelState.Key}");
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"    Error: {error.ErrorMessage}");
                        if (error.Exception != null)
                        {
                            Console.WriteLine($"    Exception: {error.Exception.Message}");
                        }
                    }
                }
            }

            // STEP 4: Save patient
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    Console.WriteLine(">>> SUCCESS! Patient saved to database <<<");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>> EXCEPTION during save: {ex.Message} <<<");
                    ModelState.AddModelError("", $"خطأ في الحفظ: {ex.Message}");
                }
            }

            Console.WriteLine(">>> FAILED! Returning to view with validation errors <<<");
            ViewBag.DoctorId = new SelectList(_context.Doctors, "Id", "Name");
            return View(patient);
        }

        // POST: Patients/CreateWithVisit (Patient + First Visit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWithVisit([Bind("Name,Phone,Age,Gender,Address")] Patient patient,
            DateTime VisitDate, string VisitType, int? DoctorId)
        {
            Console.WriteLine("====== CREATE PATIENT WITH VISIT ======");

            // STEP 1: Create PatientFile
            var patientFile = new PatientFile
            {
                Diagnosis = "ملف جديد - في انتظار التشخيص"
            };
            _context.PatientFiles.Add(patientFile);
            await _context.SaveChangesAsync();

            // STEP 2: Assign PatientFileId to patient
            patient.PatientFileId = patientFile.Id;

            // STEP 3: Clear validation
            ModelState.Remove("PatientFile");
            ModelState.Remove("Visits");
            ModelState.Remove("Diagnosis");

            if (ModelState.IsValid)
            {
                // STEP 4: Save patient
                _context.Add(patient);
                await _context.SaveChangesAsync();
                Console.WriteLine($">>> Patient saved: {patient.Name} (ID: {patient.Id})");

                // STEP 5: Create first visit if doctor selected
                if (DoctorId.HasValue && DoctorId.Value > 0)
                {
                    var visit = new Visit
                    {
                        VisitDate = VisitDate,
                        VisitType = VisitType ?? "كشف",
                        PatientId = patient.Id,
                        DoctorId = DoctorId.Value
                    };

                    // Set default diagnosis/disease if required
                    var defaultDiagnosis = await _context.Diagnoses.FirstOrDefaultAsync();
                    var defaultDisease = await _context.DiseaseTypes.FirstOrDefaultAsync();

                    if (defaultDiagnosis != null) visit.DiagnosisId = defaultDiagnosis.Id;
                    if (defaultDisease != null) visit.DiseaseTypeId = defaultDisease.Id;

                    _context.Visits.Add(visit);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($">>> First visit created for patient {patient.Name}");
                }

                Console.WriteLine(">>> SUCCESS! Patient file created");
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine(">>> FAILED! Validation errors");
            var doctors = _context.Doctors.ToList();
            ViewBag.Specializations = doctors.Select(d => d.Specialization).Distinct().ToList();
            ViewBag.DoctorsJson = doctors.Select(d => new { d.Id, d.Name, d.Specialization }).ToList();
            ViewBag.DoctorId = new SelectList(doctors, "Id", "Name", DoctorId);
            return View("Create", patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["PatientFileId"] = new SelectList(_context.PatientFiles, "Id", "Id", patient.PatientFileId);
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone,Address,Age,Gender,PatientFileId")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            // Remove validation for navigation properties and auto-generated fields
            ModelState.Remove("PatientFile");
            ModelState.Remove("PatientFileId");
            ModelState.Remove("Visits");
            ModelState.Remove("Diagnosis");
            // Also remove ID if it thinks it's 0 and invalid (though normally ID is not validated in Update if bind is correct)

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientFileId"] = new SelectList(_context.PatientFiles, "Id", "Id", patient.PatientFileId);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.PatientFile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}

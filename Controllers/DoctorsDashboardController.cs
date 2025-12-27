using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;

namespace Tadawi.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorsDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DoctorsDashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Doctor> GetCurrentDoctorAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Name == user.UserName || d.Name == user.Email);
            if (doctor == null) doctor = await _context.Doctors.FirstOrDefaultAsync();
            return doctor;
        }

        public async Task<IActionResult> Index()
        {
            var doctor = await GetCurrentDoctorAsync();
            if (doctor == null) return NotFound("لم يتم العثور على بيانات الطبيب.");

            var visits = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Prescriptions)
                .Where(v => v.DoctorId == doctor.Id)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            ViewBag.DoctorName = doctor.Name;
            return View(visits);
        }

        public async Task<IActionResult> PatientFile(int id, int visitId)
        {
            var patient = await _context.Patients
                .Include(p => p.PatientFile)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null) return NotFound();

            // Load ALL visits for this patient from ALL doctors
            var visits = await _context.Visits
                .Include(v => v.Doctor)
                .Include(v => v.Diagnosis)
                .Include(v => v.DiseaseType)
                .Include(v => v.Prescriptions)
                    .ThenInclude(p => p.Medicine)
                .Where(v => v.PatientId == id)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            ViewBag.PatientVisits = visits;
            ViewBag.CurrentVisitId = visitId;

            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> AddDiagnosis(int visitId)
        {
            var visit = await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Doctor)
                .Include(v => v.Prescriptions)
                    .ThenInclude(p => p.Medicine)
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null) return NotFound();

            ViewBag.Diseases = await _context.DiseaseTypes.ToListAsync();
            ViewBag.Medicines = await _context.Medicines.ToListAsync();
            // Pharmacies removed as per request (prescription sent to global DB)

            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiagnosis(int visitId, int diseaseTypeId,
            int[] medicineIds, string[] dosages, string[] forms, string[] durations, string[] quantities)
        {
            var visit = await _context.Visits.FindAsync(visitId);
            if (visit == null) return NotFound();

            // 1. Update Visit Diagnosis Info
            visit.DiseaseTypeId = diseaseTypeId;

            // Create or update diagnosis description
            if (visit.DiagnosisId > 0)
            {
                var diag = await _context.Diagnoses.FindAsync(visit.DiagnosisId);
                if (diag != null) diag.Description = $"تشخيص زيارة #{visitId}";
            }
            else
            {
                var diagnosis = new Diagnosis { Description = $"تشخيص زيارة #{visitId}" };
                _context.Diagnoses.Add(diagnosis);
                await _context.SaveChangesAsync();
                visit.DiagnosisId = diagnosis.Id;
            }

            // 2. Prescription Logic (Delete existing for this visit and recreate - Re-generation)
            var existingPrescriptions = _context.Prescriptions.Where(p => p.VisitId == visitId);
            _context.Prescriptions.RemoveRange(existingPrescriptions);

            if (medicineIds != null)
            {
                // Find a default pharmacy if needed for model constraints (backend not changed)
                var pharmacyId = (await _context.Pharmacies.FirstOrDefaultAsync())?.Id ?? 1;

                var random = new Random();
                for (int i = 0; i < medicineIds.Length; i++)
                {
                    if (medicineIds[i] <= 0) continue;

                    var prescriptionCode = $"RX-{DateTime.Now.Ticks % 100000}-{random.Next(100, 999)}";
                    var prescription = new Prescription
                    {
                        VisitId = visitId,
                        MedicineId = medicineIds[i],
                        PharmacyId = pharmacyId,
                        Dosage = $"[{prescriptionCode}] | {quantities[i]} | {forms[i]} | {dosages[i]} | {durations[i]}",
                        DispenseDate = new DateTime(1900, 1, 1) // Default to "Not Dispensed"
                    };
                    _context.Prescriptions.Add(prescription);
                }
            }

            _context.Update(visit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDiagnosis(int visitId)
        {
            var visit = await _context.Visits.FindAsync(visitId);
            if (visit == null) return NotFound();

            var existingPrescriptions = _context.Prescriptions.Where(p => p.VisitId == visitId);
            _context.Prescriptions.RemoveRange(existingPrescriptions);

            // Optional: reset diagnosis
            // visit.DiagnosisId = 0; 

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

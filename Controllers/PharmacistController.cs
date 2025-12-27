using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.PrescriptionRepository;
using Tadawi.Repositories.MedicineRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tadawi.Controllers
{
    [Authorize(Roles = "Pharmacist,Admin")]
    public class PharmacistController : Controller
    {
        private readonly IPrescriptionRepository _prescriptionRepo;
        private readonly IMedicineRepository _medicineRepo;
        private readonly ApplicationDbContext _context;

        public PharmacistController(IPrescriptionRepository prescriptionRepo,
                                   IMedicineRepository medicineRepo,
                                   ApplicationDbContext context)
        {
            _prescriptionRepo = prescriptionRepo;
            _medicineRepo = medicineRepo;
            _context = context;
        }

        // Dashboard Summary
        public async Task<IActionResult> Index()
        {
            var recentPrescriptions = await _context.Prescriptions
                .Include(p => p.Visit).ThenInclude(v => v.Patient)
                .Include(p => p.Medicine)
                .OrderByDescending(p => p.Id)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalMedicines = await _context.Medicines.CountAsync();
            ViewBag.PendingCount = await _context.Prescriptions
                .CountAsync(p => p.DispenseDate.Year < 2000);

            return View(recentPrescriptions);
        }

        // Inventory Management
        public async Task<IActionResult> Inventory(string? search)
        {
            var query = _context.Medicines.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.MedicineName.Contains(search) || m.Description.Contains(search));
            }
            var medicines = await query.ToListAsync();
            return View(medicines);
        }

        // Add Medicine (GET)
        public IActionResult CreateMedicine() => View();

        // Add Medicine (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMedicine(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                await _medicineRepo.AddAsync(medicine);
                TempData["Success"] = "تم إضافة الدواء للمخزون بنجاح";
                return RedirectToAction(nameof(Inventory));
            }
            return View(medicine);
        }

        // Edit Medicine (GET)
        public async Task<IActionResult> EditMedicine(int id)
        {
            var medicine = await _medicineRepo.GetByIdAsync(id);
            if (medicine == null) return NotFound();
            return View(medicine);
        }

        // Edit Medicine (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMedicine(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                await _medicineRepo.UpdateAsync(medicine);
                TempData["Success"] = "تم تحديث بيانات الدواء";
                return RedirectToAction(nameof(Inventory));
            }
            return View(medicine);
        }

        // Delete Medicine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            await _medicineRepo.DeleteAsync(id);
            TempData["Success"] = "تم حذف الدواء من المخزون";
            return RedirectToAction(nameof(Inventory));
        }

        // Scan/Search Prescription (GET)
        public IActionResult ScanCode() => View();

        // Perform Search (POST/GET)
        public async Task<IActionResult> SearchPrescription(string? prescriptionId)
        {
            if (string.IsNullOrEmpty(prescriptionId))
            {
                TempData["Error"] = "يرجى إدخال رقم الوصفة أو الكود";
                return RedirectToAction(nameof(ScanCode));
            }

            Prescription? prescription = null;

            // Try searching by ID
            if (int.TryParse(prescriptionId, out int id))
            {
                prescription = await _context.Prescriptions
                    .Include(p => p.Visit).ThenInclude(v => v.Patient)
                    .Include(p => p.Medicine)
                    .FirstOrDefaultAsync(p => p.Id == id);
            }

            // If not found by ID, try searching by the Code in Dosage (Since we save it there)
            if (prescription == null)
            {
                prescription = await _context.Prescriptions
                    .Include(p => p.Visit).ThenInclude(v => v.Patient)
                    .Include(p => p.Medicine)
                    .FirstOrDefaultAsync(p => p.Dosage.Contains(prescriptionId));
            }

            if (prescription != null)
            {
                return View("PrescriptionDetails", prescription);
            }

            TempData["Error"] = "عذراً، لم يتم العثور على وصفة بهذا الرقم أو الكود";
            return RedirectToAction(nameof(ScanCode));
        }

        // Dispense Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDispense(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription != null)
            {
                prescription.DispenseDate = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم صرف الوصفة وتغيير الحالة بنجاح";
            }
            return RedirectToAction(nameof(Index));
        }

        // Alerts View
        public async Task<IActionResult> Alerts()
        {
            // Simulate alerts: show first 5 medicines as "Low Stock" simulator
            var alerts = await _context.Medicines.Take(5).ToListAsync();
            return View(alerts);
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tadawi.Models;
using Tadawi.Repositories.PrescriptionRepository;
using Tadawi.Repositories.VisitRepository;
using Tadawi.Repositories.PharmacyRepository;
using Tadawi.Repositories.MedicineRepository;

namespace Tadawi.Controllers
{
    public class PrescriptionsController : Controller
    {
        private readonly IPrescriptionRepository _repo;
        private readonly IVisitRepository _visitRepo;
        private readonly IPharmacyRepository _pharmacyRepo;
        private readonly IMedicineRepository _medicineRepo;

        public PrescriptionsController(IPrescriptionRepository repo,
                                       IVisitRepository visitRepo,
                                       IPharmacyRepository pharmacyRepo,
                                       IMedicineRepository medicineRepo)
        {
            _repo = repo;
            _visitRepo = visitRepo;
            _pharmacyRepo = pharmacyRepo;
            _medicineRepo = medicineRepo;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAsync();
            return View(list);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var visits = await _visitRepo.GetAsync();
            var pharmacies = await _pharmacyRepo.GetAsync();
            var medicines = await _medicineRepo.GetAsync();

            ViewBag.VisitId = new SelectList(visits, "Id", "Id");
            ViewBag.PharmacyId = new SelectList(pharmacies, "Id", "Name");
            ViewBag.MedicineId = new SelectList(medicines, "Id", "MedicineName");

            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string dosage, int visitId, int pharmacyId, int medicineId, DateTime? dispenseDate)
        {
            if (!ModelState.IsValid)
                return await Create();

            var success = await _repo.AddAsync(new Prescription
            {
                Dosage = dosage,
                VisitId = visitId,
                PharmacyId = pharmacyId,
                MedicineId = medicineId,
                DispenseDate = dispenseDate ?? DateTime.Today
            });

            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to save.");
            return await Create();
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null) return NotFound();

            var visits = await _visitRepo.GetAsync();
            var pharmacies = await _pharmacyRepo.GetAsync();
            var medicines = await _medicineRepo.GetAsync();

            ViewBag.VisitId = new SelectList(visits, "Id", "Id", prescription.VisitId);
            ViewBag.PharmacyId = new SelectList(pharmacies, "Id", "Name", prescription.PharmacyId);
            ViewBag.MedicineId = new SelectList(medicines, "Id", "MedicineName", prescription.MedicineId);

            return View(prescription);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string dosage, int visitId, int pharmacyId, int medicineId, DateTime dispenseDate)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null) return NotFound();

            prescription.Dosage = dosage;
            prescription.VisitId = visitId;
            prescription.PharmacyId = pharmacyId;
            prescription.MedicineId = medicineId;
            prescription.DispenseDate = dispenseDate;

            var success = await _repo.UpdateAsync(prescription);
            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update.");
            return await Edit(id);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _repo.GetByIdAsync(id);
            if (prescription == null) return NotFound();
            return View(prescription);
        }

        // POST: DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tadawi.Models;
using Tadawi.Repositories.MedicineRepository;

namespace Tadawi.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly IMedicineRepository _medicineRepository;

        public MedicinesController(IMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        // GET: Medicines
        public async Task<IActionResult> Index()
        {
            var medicines = await _medicineRepository.GetAsync();
            return View(medicines);
        }

        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var medicine = await _medicineRepository.GetByIdAsync(id.Value);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MedicineName,Description,Type")] Medicine medicine)
        {
            if (await _medicineRepository.AddAsync(medicine))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(medicine);
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medicine = await _medicineRepository.GetByIdAsync(id.Value);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // POST: Medicines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MedicineName,Description,Type")] Medicine medicine)
        {
            if (id != medicine.Id) return NotFound();

            if (await _medicineRepository.UpdateAsync(medicine))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(medicine);
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var medicine = await _medicineRepository.GetByIdAsync(id.Value);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicineRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tadawi.Models;
using Tadawi.Repositories.DiagnosisRepository;

namespace Tadawi.Controllers
{
    public class DiagnosisController : Controller
    {
        private readonly IDiagnosisRepository _diagnosisRepository;

        public DiagnosisController(IDiagnosisRepository diagnosisRepository)
        {
            _diagnosisRepository = diagnosisRepository;
        }

        // GET: Diagnosis
        public async Task<IActionResult> Index()
        {
            return View(await _diagnosisRepository.GetAsync());
        }

        // GET: Diagnosis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var diagnosis = await _diagnosisRepository.GetByIdAsync(id.Value);
            if (diagnosis == null)
            {
                return NotFound();
            }
            return View(diagnosis);
        }

        // GET: Diagnosis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diagnosis/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description")] Diagnosis diagnosis)
        {
            if (ModelState.IsValid)
            {
                if (await _diagnosisRepository.AddAsync(diagnosis))
                    return RedirectToAction(nameof(Index));
            }
            return View(diagnosis);
        }

        // GET: Diagnosis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var diagnosis = await _diagnosisRepository.GetByIdAsync(id.Value);
            if (diagnosis == null)
                return NotFound();

            return View(diagnosis);
        }

        // POST: Diagnosis/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description")] Diagnosis diagnosis)
        {
            if (id != diagnosis.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (await _diagnosisRepository.UpdateAsync(diagnosis))
                    return RedirectToAction(nameof(Index));
            }

            return View(diagnosis);
        }

        // GET: Diagnosis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var diagnosis = await _diagnosisRepository.GetByIdAsync(id.Value);
            if (diagnosis == null)
            {
                return NotFound();
            }
            return View(diagnosis);
        }

        // POST: Diagnosis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diagnosis = await _diagnosisRepository.GetByIdAsync(id);
            if (await _diagnosisRepository.DeleteAsync(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(diagnosis);
        }
    }
}

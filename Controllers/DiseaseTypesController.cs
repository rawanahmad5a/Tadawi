using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.DiseaseTypeRepository;

namespace Tadawi.Controllers
{
    public class DiseaseTypesController : Controller
    {
        private readonly IDiseaseTypeRepository _diseaseTypeRepository;

        public DiseaseTypesController(IDiseaseTypeRepository diseaseTypeRepository)
        {
            _diseaseTypeRepository = diseaseTypeRepository;
        }

        // GET: DiseaseTypes
        public async Task<IActionResult> Index()
        {
            return View(await _diseaseTypeRepository.GetAsync());
        }

        // GET: DiseaseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diseaseType = await _diseaseTypeRepository.GetByIdAsync(id.Value);

            if (diseaseType == null)
            {
                return NotFound();
            }

            return View(diseaseType);
        }

        // GET: DiseaseTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DiseaseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DiseaseName,Symptoms")] DiseaseType diseaseType)
        {
            if (ModelState.IsValid)
            {
                if(await _diseaseTypeRepository.AddAsync(diseaseType))
                {
                    return RedirectToAction(nameof(Index));
                }
                
            }
            return View(diseaseType);
        }

        // GET: DiseaseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diseaseType = await _diseaseTypeRepository.GetByIdAsync(id.Value);
            if (diseaseType == null)
            {
                return NotFound();
            }
            return View(diseaseType);
        }

        // POST: DiseaseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DiseaseName,Symptoms")] DiseaseType diseaseType)
        {
            if (id != diseaseType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(await _diseaseTypeRepository.UpdateAsync(diseaseType))
                {
                    return RedirectToAction(nameof(Index)); 
                }
            }
            return View(diseaseType);
        }

        // GET: DiseaseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diseaseType = await _diseaseTypeRepository.GetByIdAsync(id.Value);
            if (diseaseType == null)
            {
                return NotFound();
            }

            return View(diseaseType);
        }

        // POST: DiseaseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diseaseType = await _diseaseTypeRepository.GetByIdAsync(id);
            if(await _diseaseTypeRepository.DeleteAsync(id))
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(diseaseType);
        }
    }
}

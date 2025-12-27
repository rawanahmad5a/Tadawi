using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using Tadawi.Repositories.PeriodicDispenseRepository;

namespace Tadawi.Controllers
{
    public class PeriodicDispensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPeriodicDispenseRepository _repository;

        public PeriodicDispensesController(
            ApplicationDbContext context,
            IPeriodicDispenseRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var data = _context.PeriodicDispenses
                .Include(x => x.Patient)
                .Include(x => x.Medicine);

            return View(await data.ToListAsync());
        }

        // GET Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name");
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "Id", "MedicineName");
            return View();
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PeriodicDispenseId,Frequency,NextDispenseDate,PatientId,MedicineId")]
            PeriodicDispense periodicDispense)
        {
            if (await _repository.AddAsync(periodicDispense))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "Name", periodicDispense.PatientId);
            ViewData["MedicineId"] = new SelectList(_context.Medicines, "Id", "MedicineName", periodicDispense.MedicineId);
            return View(periodicDispense);
        }
    }
}

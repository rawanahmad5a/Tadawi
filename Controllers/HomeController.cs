using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tadawi.Models;

namespace Tadawi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");

            if (User.IsInRole("Doctor"))
                return RedirectToAction("Index", "DoctorsDashboard");

            if (User.IsInRole("Pharmacist"))
                return RedirectToAction("Index", "Pharmacist");

            if (User.IsInRole("Secretary"))
                return RedirectToAction("Index", "Patients");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

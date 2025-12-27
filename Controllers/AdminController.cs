using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tadawi.Data;
using Tadawi.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Tadawi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Dashboard Statistics
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "بدون صلاحية";
            }

            ViewBag.TotalUsers = users.Count;
            ViewBag.TotalDoctors = await _context.Doctors.CountAsync();
            ViewBag.TotalPharmacies = await _context.Pharmacies.CountAsync();
            ViewBag.TotalOthers = userRoles.Values.Count(r => r == "Secretary" || r == "Pharmacist");

            return View();
        }

        // Users Management List
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.FirstOrDefault() ?? "بدون صلاحية";
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // Create User (GET)
        public async Task<IActionResult> CreateUser()
        {
            string[] roles = { "Admin", "Doctor", "Pharmacist", "Secretary" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }

            ViewBag.Roles = new SelectList(roles);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string? email, string? password, string? role, string? phone, string? fullName, string? specialization)
        {
            // Even though we have parameters, we can still use Request.Form for extra safety 
            // but let's just use the parameters now that we've cleared ModelState.
            email ??= Request.Form["email"];
            password ??= Request.Form["password"];
            role ??= Request.Form["role"];
            phone ??= Request.Form["phone"];
            fullName ??= Request.Form["fullName"];
            specialization ??= Request.Form["specialization"];

            ModelState.Clear(); // Critical: Start fresh

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "جميع الحقول الأساسية مطلوبة (البريد الإلكتروني، كلمة المرور، الدور)");

                // Debugging: If these are somehow empty, we need to know why
                if (string.IsNullOrEmpty(email)) ModelState.AddModelError("email", "البريد الإلكتروني مفقود من النموذج");
                if (string.IsNullOrEmpty(password)) ModelState.AddModelError("password", "كلمة المرور مفقودة من النموذج");
                if (string.IsNullOrEmpty(role)) ModelState.AddModelError("role", "الدور مفقود من النموذج");
            }

            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(email!);
                if (userExists != null)
                {
                    ModelState.AddModelError("", "هذا البريد الإلكتروني مسجل مسبقاً");
                }
                else
                {
                    var user = new IdentityUser { UserName = email, Email = email, PhoneNumber = phone, EmailConfirmed = true };
                    var result = await _userManager.CreateAsync(user, password!);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, role!);

                        if (role == "Doctor")
                        {
                            var doctor = new Doctor
                            {
                                Name = !string.IsNullOrEmpty(fullName) ? fullName : email!,
                                Phone = phone ?? "0",
                                Specialization = !string.IsNullOrEmpty(specialization) ? specialization : "عام"
                            };
                            _context.Doctors.Add(doctor);
                        }

                        await _context.SaveChangesAsync();
                        TempData["Success"] = "تم إنشاء الحساب بنجاح بنسبة 100%";
                        return RedirectToAction(nameof(Users));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Doctor", "Pharmacist", "Secretary" });
            return View();
        }

        // Edit User (GET)
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = roles.FirstOrDefault();
            ViewBag.Roles = new SelectList(new[] { "Admin", "Doctor", "Pharmacist", "Secretary" });

            return View(user);
        }

        // Edit User (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string id, string email, string phone, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            user.UserName = email;
            user.PhoneNumber = phone;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, role);

                return RedirectToAction(nameof(Users));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.Roles = new SelectList(new[] { "Admin", "Doctor", "Pharmacist", "Secretary" });
            return View(user);
        }

        // Deactivate User (Toggle)
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Toggle lockout as a way to "deactivate"
                if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                }
            }
            return RedirectToAction(nameof(Users));
        }

        // Delete User
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        // Pharmacies Management List
        public async Task<IActionResult> Pharmacies()
        {
            var pharmacies = await _context.Pharmacies.ToListAsync();

            // Map pharmacy status by checking for an associated IdentityUser
            var pharmacyStatuses = new Dictionary<int, bool>();
            foreach (var p in pharmacies)
            {
                // Consistent search logic with DeactivatePharmacy
                var user = await _userManager.Users.FirstOrDefaultAsync(u =>
                    u.PhoneNumber == p.Phone ||
                    u.Email!.Contains(p.Name) ||
                    u.UserName!.Contains(p.Name));

                // If user doesn't exist or is not locked out, we consider it "Active" (true)
                bool isActive = user == null || user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now;
                pharmacyStatuses[p.Id] = isActive;
            }

            ViewBag.PharmacyStatuses = pharmacyStatuses;
            return View(pharmacies);
        }

        // Create Pharmacy (GET)
        public IActionResult CreatePharmacy()
        {
            return View();
        }

        // Create Pharmacy (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePharmacy(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                _context.Pharmacies.Add(pharmacy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Pharmacies));
            }
            return View(pharmacy);
        }

        // Edit Pharmacy (GET)
        public async Task<IActionResult> EditPharmacy(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy == null) return NotFound();
            return View(pharmacy);
        }

        // Edit Pharmacy (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPharmacy(int id, Pharmacy pharmacy)
        {
            if (id != pharmacy.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(pharmacy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Pharmacies));
            }
            return View(pharmacy);
        }

        // Delete Pharmacy
        public async Task<IActionResult> DeletePharmacy(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy != null)
            {
                _context.Pharmacies.Remove(pharmacy);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الصيدلية بنجاح";
            }
            return RedirectToAction(nameof(Pharmacies));
        }

        // Deactivate Pharmacy (Mechanism: Find associated Pharmacist user by Name/Phone and toggle lockout)
        public async Task<IActionResult> DeactivatePharmacy(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy != null)
            {
                // Try to find a user linked to this pharmacy (by phone or by matching email/username)
                var associatedUser = await _userManager.Users.FirstOrDefaultAsync(u =>
                    u.PhoneNumber == pharmacy.Phone ||
                    u.Email!.Contains(pharmacy.Name) ||
                    u.UserName!.Contains(pharmacy.Name));

                if (associatedUser != null)
                {
                    if (associatedUser.LockoutEnd == null || associatedUser.LockoutEnd < DateTime.Now)
                    {
                        await _userManager.SetLockoutEnabledAsync(associatedUser, true);
                        await _userManager.SetLockoutEndDateAsync(associatedUser, DateTimeOffset.MaxValue);
                        TempData["Success"] = $"تم إيقاف صلاحيات الصيدلية {pharmacy.Name} بنجاح";
                    }
                    else
                    {
                        await _userManager.SetLockoutEndDateAsync(associatedUser, null);
                        TempData["Success"] = $"تم تفعيل صلاحيات الصيدلية {pharmacy.Name} بنجاح";
                    }
                }
                else
                {
                    TempData["Error"] = "لم يتم العثور على حساب مستخدم مرتبط بهذه الصيدلية لإيقافه.";
                }
            }
            return RedirectToAction(nameof(Pharmacies));
        }

        // Settings View
        public IActionResult Settings()
        {
            return View();
        }
    }
}
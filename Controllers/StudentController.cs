using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SchoolApp.Data;
using SchoolApp.Models;
using SchoolApp.ViewModels;
using System.Security.Claims;

namespace SchoolApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _db;

        public StudentController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Student/Register
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true &&
                User.FindFirst("UserType")?.Value == "Student")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        // POST: /Student/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentRegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_db.Students.Any(s => s.Email == model.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var student = new Student
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
                RollNumber = model.RollNumber,
                EnrollmentYear = model.EnrollmentYear,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Student/Login
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true &&
                User.FindFirst("UserType")?.Value == "Student")
            {
                return RedirectToAction(nameof(Dashboard));
            }

            return View();
        }

        // POST: /Student/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(StudentLoginVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var student = _db.Students.FirstOrDefault(s => s.Email == model.Email);

            if (student == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.Password, student.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Name, student.FullName),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim("UserType", "Student")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });

            return RedirectToAction(nameof(Dashboard));
        }

        // GET: /Student/Dashboard
        public IActionResult Dashboard()
        {
            if (!User.Identity!.IsAuthenticated ||
                User.FindFirst("UserType")?.Value != "Student")
            {
                return RedirectToAction(nameof(Login));
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idClaim))
                return RedirectToAction(nameof(Login));

            var student = _db.Students.Find(int.Parse(idClaim));

            if (student == null)
                return RedirectToAction(nameof(Login));

            return View(student);
        }

        // POST: /Student/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}

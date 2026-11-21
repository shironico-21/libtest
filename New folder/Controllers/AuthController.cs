using finalProject.Dto;
using finalProject.Service.interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finalProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAutthContract _authService;

        public AuthController(IAutthContract authService)
        {
            _authService = authService;
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AuthUserDtos authUser)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. The email might already be in use.");
                return View(authUser);
            }

            var result = _authService.Register(authUser);
            if (result)
            {
                
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. The email  already be in use.");
            return View(authUser);
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(loginDtos login) 
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            // 1. Get the user from your service
            var user = _authService.login(login);

            if (user != null)
            {
                // 2. Build the Identity Claims (This tells the system WHO they are and WHAT ROLE they have)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    // MAGIC HAPPENS HERE:
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                };

                // Change these two lines inside your Login method:

                var identity = new ClaimsIdentity(claims, "Identity.Application");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Identity.Application", principal);

                return RedirectToAction("Index", "Hotel");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(login);
        }

        // POST: /Auth/Logout
        // Note: Logout does not need a GET View. It should be triggered by a form POST in your layout.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Change the logout scheme here as well:
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index", "Home");
        }
    }
}
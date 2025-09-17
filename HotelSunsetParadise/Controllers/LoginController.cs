using HotelSunsetParadise.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelSunsetParadise.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Usuario y contraseña hardcodeados como se solicita
                if (model.Usuario == "admin" && model.Contrasena == "123")
                {
                    // Usar ViewBag para mantener el estado de login
                    ViewBag.LoggedIn = true;
                    HttpContext.Session.SetString("IsLoggedIn", "true");

                    return RedirectToAction("Index", "Reservas");
                }
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}

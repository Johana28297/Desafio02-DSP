using HotelSunsetParadise.Data;
using HotelSunsetParadise.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelSunsetParadise.Controllers
{
    public class HabitacionesController : Controller
    {
        public IActionResult Index()
        {
            return View(DatosHotel.Habitaciones);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Habitacion habitacion)
        {
            // *** Coloca el punto de interrupción en la siguiente línea ***
            if (ModelState.IsValid)
            {
                // Si el modelo es válido, se ejecuta la lógica para guardar la habitación.
                habitacion.Id = DatosHotel.GetNextHabitacionId();
                habitacion.Estado = "Disponible";
                DatosHotel.Habitaciones.Add(habitacion);
                return RedirectToAction("Index");
            }

            // Si el modelo no es válido, se devuelve la vista con los errores.
            return View(habitacion);
        }
    }
}
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

        // GET: Habitaciones/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == id);
            if (habitacion == null)
            {
                return NotFound();
            }

            return View(habitacion);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Habitacion habitacion)
        {
            if (ModelState.IsValid)
            {
                // Validar que no exista otra habitación con el mismo número
                var habitacionExistente = DatosHotel.Habitaciones.FirstOrDefault(h => h.Numero == habitacion.Numero);
                if (habitacionExistente != null)
                {
                    ModelState.AddModelError("Numero", "Ya existe una habitación con este número.");
                    return View(habitacion);
                }

                habitacion.Id = DatosHotel.GetNextHabitacionId();
                habitacion.Estado = "Disponible";
                DatosHotel.Habitaciones.Add(habitacion);
                return RedirectToAction("Index");
            }
            return View(habitacion);
        }

        // GET: Habitaciones/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == id);
            if (habitacion == null)
            {
                return NotFound();
            }

            return View(habitacion);
        }

        // POST: Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Habitacion habitacion)
        {
            if (id != habitacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que no exista otra habitación con el mismo número (excluyendo la habitación actual)
                    var habitacionConMismoNumero = DatosHotel.Habitaciones.FirstOrDefault(h => h.Numero == habitacion.Numero && h.Id != habitacion.Id);
                    if (habitacionConMismoNumero != null)
                    {
                        ModelState.AddModelError("Numero", "Ya existe otra habitación con este número.");
                        return View(habitacion);
                    }

                    var habitacionExistente = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == habitacion.Id);
                    if (habitacionExistente != null)
                    {
                        habitacionExistente.Numero = habitacion.Numero;
                        habitacionExistente.Tipo = habitacion.Tipo;
                        habitacionExistente.PrecioPorNoche = habitacion.PrecioPorNoche;
                        habitacionExistente.Estado = habitacion.Estado;

                        return RedirectToAction("Index");
                    }
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la habitación.");
                }
            }

            return View(habitacion);
        }

        // GET: Habitaciones/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == id);
            if (habitacion == null)
            {
                return NotFound();
            }

            return View(habitacion);
        }

        // POST: Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == id);
            if (habitacion != null)
            {
                // Verificar si la habitación tiene reservas activas
                var tieneReservas = DatosHotel.Reservas.Any(r => r.HabitacionId == id);
                if (tieneReservas)
                {
                    TempData["Error"] = "No se puede eliminar la habitación porque tiene reservas asociadas.";
                    return RedirectToAction("Delete", new { id = id });
                }

                DatosHotel.Habitaciones.Remove(habitacion);
            }

            return RedirectToAction("Index");
        }
    }
}
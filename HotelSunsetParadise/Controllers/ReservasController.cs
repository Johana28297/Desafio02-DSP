using HotelSunsetParadise.Data;
using HotelSunsetParadise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelSunsetParadise.Controllers
{
    public class ReservasController : Controller
    {
        public IActionResult Index()
        {
            var reservas = DatosHotel.Reservas.Select(r => new Reserva
            {
                Id = r.Id,
                Cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == r.ClienteId),
                Habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == r.HabitacionId),
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                CostoTotal = r.CostoTotal
            }).ToList();
            return View(reservas);
        }

        public IActionResult Create()
        {
            CargarViewBags();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Reserva reserva)
        {
            // Validar la lógica de negocio de fechas
            if (reserva.FechaSalida <= reserva.FechaEntrada)
            {
                ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada.");
            }

            // Validar solapamiento de fechas
            var habitacionOcupada = DatosHotel.Reservas.Any(r => r.HabitacionId == reserva.HabitacionId &&
                                                                 (reserva.FechaEntrada < r.FechaSalida && reserva.FechaSalida > r.FechaEntrada));
            if (habitacionOcupada)
            {
                ModelState.AddModelError("HabitacionId", "La habitación ya está reservada en esas fechas.");
            }

            if (ModelState.IsValid)
            {
                // Lógica para guardar la reserva
                var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reserva.HabitacionId);
                if (habitacion != null)
                {
                    var totalNoches = (reserva.FechaSalida - reserva.FechaEntrada).Days;
                    reserva.CostoTotal = totalNoches * habitacion.PrecioPorNoche;

                    reserva.Id = DatosHotel.GetNextReservaId();
                    DatosHotel.Reservas.Add(reserva);

                    var hab = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reserva.HabitacionId);
                    if (hab != null)
                    {
                        hab.Estado = "Ocupada";
                    }
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "La habitación seleccionada no es válida.");
            }

            CargarViewBags(reserva.ClienteId, reserva.HabitacionId);
            return View(reserva);
        }

        private void CargarViewBags(int? clienteId = null, int? habitacionId = null)
        {
            ViewBag.Clientes = new SelectList(DatosHotel.Clientes, "Id", "Nombre", clienteId);
            ViewBag.Habitaciones = new SelectList(DatosHotel.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", habitacionId);
        }
    }
}
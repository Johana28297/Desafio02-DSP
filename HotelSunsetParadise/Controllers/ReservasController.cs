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
                ClienteId = r.ClienteId,
                HabitacionId = r.HabitacionId,
                Cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == r.ClienteId),
                Habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == r.HabitacionId),
                FechaEntrada = r.FechaEntrada,
                FechaSalida = r.FechaSalida,
                CostoTotal = r.CostoTotal
            }).ToList();
            return View(reservas);
        }

        // GET: Reservas/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaData = DatosHotel.Reservas.FirstOrDefault(r => r.Id == id);
            if (reservaData == null)
            {
                return NotFound();
            }

            var reserva = new Reserva
            {
                Id = reservaData.Id,
                ClienteId = reservaData.ClienteId,
                HabitacionId = reservaData.HabitacionId,
                Cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == reservaData.ClienteId),
                Habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reservaData.HabitacionId),
                FechaEntrada = reservaData.FechaEntrada,
                FechaSalida = reservaData.FechaSalida,
                CostoTotal = reservaData.CostoTotal
            };

            return View(reserva);
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

        // GET: Reservas/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaData = DatosHotel.Reservas.FirstOrDefault(r => r.Id == id);
            if (reservaData == null)
            {
                return NotFound();
            }

            var reserva = new Reserva
            {
                Id = reservaData.Id,
                ClienteId = reservaData.ClienteId,
                HabitacionId = reservaData.HabitacionId,
                FechaEntrada = reservaData.FechaEntrada,
                FechaSalida = reservaData.FechaSalida,
                CostoTotal = reservaData.CostoTotal
            };

            CargarViewBagsParaEdicion(reserva.ClienteId, reserva.HabitacionId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            // Validar la lógica de negocio de fechas
            if (reserva.FechaSalida <= reserva.FechaEntrada)
            {
                ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada.");
            }

            // Validar solapamiento de fechas (excluyendo la reserva actual)
            var habitacionOcupada = DatosHotel.Reservas.Any(r => r.Id != reserva.Id &&
                                                                 r.HabitacionId == reserva.HabitacionId &&
                                                                 (reserva.FechaEntrada < r.FechaSalida && reserva.FechaSalida > r.FechaEntrada));
            if (habitacionOcupada)
            {
                ModelState.AddModelError("HabitacionId", "La habitación ya está reservada en esas fechas.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var reservaExistente = DatosHotel.Reservas.FirstOrDefault(r => r.Id == reserva.Id);
                    if (reservaExistente != null)
                    {
                        // Si cambió de habitación, liberar la anterior y ocupar la nueva
                        if (reservaExistente.HabitacionId != reserva.HabitacionId)
                        {
                            // Liberar habitación anterior
                            var habitacionAnterior = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reservaExistente.HabitacionId);
                            if (habitacionAnterior != null)
                            {
                                habitacionAnterior.Estado = "Disponible";
                            }

                            // Ocupar nueva habitación
                            var nuevaHabitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reserva.HabitacionId);
                            if (nuevaHabitacion != null)
                            {
                                nuevaHabitacion.Estado = "Ocupada";
                            }
                        }

                        // Recalcular costo si es necesario
                        var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reserva.HabitacionId);
                        if (habitacion != null)
                        {
                            var totalNoches = (reserva.FechaSalida - reserva.FechaEntrada).Days;
                            reserva.CostoTotal = totalNoches * habitacion.PrecioPorNoche;
                        }

                        // Actualizar los datos
                        reservaExistente.ClienteId = reserva.ClienteId;
                        reservaExistente.HabitacionId = reserva.HabitacionId;
                        reservaExistente.FechaEntrada = reserva.FechaEntrada;
                        reservaExistente.FechaSalida = reserva.FechaSalida;
                        reservaExistente.CostoTotal = reserva.CostoTotal;

                        return RedirectToAction("Index");
                    }
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la reserva.");
                }
            }

            CargarViewBagsParaEdicion(reserva.ClienteId, reserva.HabitacionId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservaData = DatosHotel.Reservas.FirstOrDefault(r => r.Id == id);
            if (reservaData == null)
            {
                return NotFound();
            }

            var reserva = new Reserva
            {
                Id = reservaData.Id,
                ClienteId = reservaData.ClienteId,
                HabitacionId = reservaData.HabitacionId,
                Cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == reservaData.ClienteId),
                Habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reservaData.HabitacionId),
                FechaEntrada = reservaData.FechaEntrada,
                FechaSalida = reservaData.FechaSalida,
                CostoTotal = reservaData.CostoTotal
            };

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var reserva = DatosHotel.Reservas.FirstOrDefault(r => r.Id == id);
            if (reserva != null)
            {
                // Liberar la habitación
                var habitacion = DatosHotel.Habitaciones.FirstOrDefault(h => h.Id == reserva.HabitacionId);
                if (habitacion != null)
                {
                    habitacion.Estado = "Disponible";
                }

                // Eliminar la reserva
                DatosHotel.Reservas.Remove(reserva);
            }

            return RedirectToAction("Index");
        }

        private void CargarViewBags(int? clienteId = null, int? habitacionId = null)
        {
            ViewBag.Clientes = new SelectList(DatosHotel.Clientes, "Id", "Nombre", clienteId);
            ViewBag.Habitaciones = new SelectList(DatosHotel.Habitaciones.Where(h => h.Estado == "Disponible"), "Id", "Numero", habitacionId);
        }

        private void CargarViewBagsParaEdicion(int? clienteId = null, int? habitacionId = null)
        {
            ViewBag.Clientes = new SelectList(DatosHotel.Clientes, "Id", "Nombre", clienteId);
            // Para edición, incluimos todas las habitaciones (incluyendo la actual que puede estar ocupada)
            ViewBag.Habitaciones = new SelectList(DatosHotel.Habitaciones, "Id", "Numero", habitacionId);
        }
    }
}
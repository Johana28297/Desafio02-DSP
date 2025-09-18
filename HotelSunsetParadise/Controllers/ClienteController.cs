using HotelSunsetParadise.Data;
using HotelSunsetParadise.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelSunsetParadise.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View(DatosHotel.Clientes);
        }

        // GET: Clientes/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Validar que no exista otro cliente con el mismo DUI
                var clienteExistente = DatosHotel.Clientes.FirstOrDefault(c => c.DUI == cliente.DUI);
                if (clienteExistente != null)
                {
                    ModelState.AddModelError("DUI", "Ya existe un cliente registrado con este DUI.");
                    return View(cliente);
                }

                cliente.Id = DatosHotel.GetNextClienteId();
                DatosHotel.Clientes.Add(cliente);
                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que no exista otro cliente con el mismo DUI (excluyendo el cliente actual)
                    var clienteConMismoDUI = DatosHotel.Clientes.FirstOrDefault(c => c.DUI == cliente.DUI && c.Id != cliente.Id);
                    if (clienteConMismoDUI != null)
                    {
                        ModelState.AddModelError("DUI", "Ya existe otro cliente registrado con este DUI.");
                        return View(cliente);
                    }

                    var clienteExistente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == cliente.Id);
                    if (clienteExistente != null)
                    {
                        clienteExistente.Nombre = cliente.Nombre;
                        clienteExistente.DUI = cliente.DUI;
                        clienteExistente.Telefono = cliente.Telefono;

                        return RedirectToAction("Index");
                    }
                    return NotFound();
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el cliente.");
                }
            }

            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var cliente = DatosHotel.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente != null)
            {
                // Verificar si el cliente tiene reservas activas
                var tieneReservas = DatosHotel.Reservas.Any(r => r.ClienteId == id);
                if (tieneReservas)
                {
                    TempData["Error"] = "No se puede eliminar el cliente porque tiene reservas asociadas.";
                    return RedirectToAction("Delete", new { id = id });
                }

                DatosHotel.Clientes.Remove(cliente);
            }

            return RedirectToAction("Index");
        }
    }
}
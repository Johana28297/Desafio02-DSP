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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.Id = DatosHotel.GetNextClienteId();
                DatosHotel.Clientes.Add(cliente);
                return RedirectToAction("Index");
            }
            return View(cliente);
        }
    }
}

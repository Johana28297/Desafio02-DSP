using System.Collections.Generic;
using HotelSunsetParadise.Models;

namespace HotelSunsetParadise.Data
{
    public static class DatosHotel
    {
        public static List<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
        public static List<Cliente> Clientes { get; set; } = new List<Cliente>();
        public static List<Reserva> Reservas { get; set; } = new List<Reserva>();

        static DatosHotel()
        {
            // Pre-poblar con datos de ejemplo
            Habitaciones.Add(new Habitacion { Id = 1, Numero = 101, Tipo = "Individual", PrecioPorNoche = 50.00, Estado = "Disponible" });
            Habitaciones.Add(new Habitacion { Id = 2, Numero = 102, Tipo = "Doble", PrecioPorNoche = 75.00, Estado = "Disponible" });
            Habitaciones.Add(new Habitacion { Id = 3, Numero = 103, Tipo = "Suite", PrecioPorNoche = 150.00, Estado = "Ocupada" });

            Clientes.Add(new Cliente { Id = 1, Nombre = "Juan Pérez", DUI = "12345678-9", Telefono = "7777-1111" });
            Clientes.Add(new Cliente { Id = 2, Nombre = "María López", DUI = "98765432-1", Telefono = "7777-2222" });
        }

        public static int GetNextHabitacionId()
        {
            if (Habitaciones.Count == 0) return 1;
            return Habitaciones[^1].Id + 1;
        }

        public static int GetNextClienteId()
        {
            if (Clientes.Count == 0) return 1;
            return Clientes[^1].Id + 1;
        }

        public static int GetNextReservaId()
        {
            if (Reservas.Count == 0) return 1;
            return Reservas[^1].Id + 1;
        }
    }
}
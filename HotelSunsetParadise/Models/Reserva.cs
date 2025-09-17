using System;
using System.ComponentModel.DataAnnotations;

namespace HotelSunsetParadise.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; } // Propiedad que acepta valor nulo

        [Required]
        public int HabitacionId { get; set; }
        public Habitacion? Habitacion { get; set; } // Propiedad que acepta valor nulo

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaEntrada { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaSalida { get; set; }

        public double CostoTotal { get; set; }
    }
}
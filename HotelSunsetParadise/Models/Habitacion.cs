namespace HotelSunsetParadise.Models
{
    public class Habitacion
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public string? Tipo { get; set; }
        public double PrecioPorNoche { get; set; }
        public string? Estado { get; set; } // Ejemplo: "Disponible", "Ocupada"
    }
}

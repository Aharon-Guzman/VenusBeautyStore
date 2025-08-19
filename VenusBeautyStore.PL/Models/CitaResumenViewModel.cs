using System;
using System.Collections.Generic;

namespace VenusBeautyStore.PL.Models
{
    public class CitaResumenViewModel
    {
        public int IdCita { get; set; }
        public DateTime? FechaHora { get; set; }
        public string? NombreTrabajador { get; set; }
        public decimal Total { get; set; }
        public List<ItemServicioVM> Servicios { get; set; } = new();
        public List<ItemProductoVM> Productos { get; set; } = new();
    }

    public class ItemServicioVM
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }

    public class ItemProductoVM
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}

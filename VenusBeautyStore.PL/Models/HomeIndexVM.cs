using System.Collections.Generic;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Models
{
    public class HomeIndexVM
    {
        public List<VenusBeauty.DAL.Entities.Producto> Productos { get; set; } = new();
        public List<VenusBeauty.DAL.Entities.Servicio> Servicios { get; set; } = new();
    }
}

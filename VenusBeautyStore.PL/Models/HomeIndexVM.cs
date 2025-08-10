using System.Collections.Generic;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Models
{
    public class HomeIndexVM
    {
        public List<Producto> Productos { get; set; } = new();
    }
}

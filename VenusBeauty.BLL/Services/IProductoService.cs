using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.BLL.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerProductosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<bool> CrearProductoAsync(Producto producto);
        Task<bool> EditarProductoAsync(int id, Producto producto);
        Task<bool> EliminarProductoAsync(int id);
        Task<bool> CambiarEstadoAsync(int id);
        Task<IEnumerable<Producto>> ObtenerProductosActivosAsync();

    }
}

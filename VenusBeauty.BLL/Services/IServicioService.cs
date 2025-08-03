using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.BLL.Services
{
    public interface IServicioService
    {
        Task<IEnumerable<Servicio>> ObtenerServiciosAsync();
        Task<Servicio?> ObtenerPorIdAsync(int id);
        Task<bool> CrearServicioAsync(Servicio servicio);
        Task<bool> EditarServicioAsync(int id, Servicio servicio);
        Task<bool> EliminarServicioAsync(int id);
        Task<bool> CambiarEstadoAsync(int id);
        Task<IEnumerable<Servicio>> ObtenerServiciosActivosAsync();

    }
}

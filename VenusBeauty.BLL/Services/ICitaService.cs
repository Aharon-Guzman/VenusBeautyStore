using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.BLL.Services
{
    public interface ICitaService
    {
        Task<Cita?> ObtenerCitaAsync(int id);
        Task<IEnumerable<Cita>> ObtenerAgendaAsync(
            DateTime desde, DateTime hasta, string? idUsuario = null);

        Task<int> CrearCitaAsync(
            int idCliente,
            string idUsuario,
            DateTime fechaHora,
            IEnumerable<int> idServicios,
            IDictionary<int, int>? productos = null); // productoId -> cantidad

        Task<bool> ActualizarEstadoAsync(int idCita, EstadoCita nuevoEstado);
        Task<bool> CancelarCitaAsync(int idCita);
        Task<bool> EliminarCitaAsync(int idCita);
        //nuevo
        Task<int?> ResolverCitaClienteAsync(string userId, int? idCita);
        Task<IEnumerable<Cita>> ObtenerCitasDelClienteAsync(string userId, bool soloAbiertas = true);
        Task AgregarServicioAsync(int idCita, int idServicio, string userId);
        Task AgregarProductoAsync(int idCita, int idProducto, int cantidad, string userId);

        Task QuitarServicioAsync(int idCita, int idServicio, string userId);
        Task QuitarProductoAsync(int idCita, int idProducto, string userId);
        Task ActualizarCantidadProductoAsync(int idCita, int idProducto, int nuevaCantidad, string userId);


    }
}

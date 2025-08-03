using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.BLL.Services
{
    public interface ITrabajadorService
    {
        Task<IEnumerable<Trabajador>> ObtenerTrabajadoresAsync();
        Task<Trabajador?> ObtenerPorIdAsync(int id);

        // 🔹 NUEVO: Crear con email y errores detallados
        Task<(bool Succeeded, IEnumerable<string> Errors)> CrearTrabajadorAsync(
            Trabajador trabajador,
            string password,
            string rol,
            string email
        );

        Task<bool> EditarTrabajadorAsync(int id, Trabajador trabajador);
        Task<bool> EliminarTrabajadorAsync(int id);
        Task<bool> CambiarPasswordAsync(string userId, string newPassword);
        Task<(bool Succeeded, IEnumerable<string> Errors)> CrearTrabajadorConErroresAsync(Trabajador trabajador, string password, string rol, string email);
        Task<IEnumerable<Trabajador>> ObtenerEstilistasAsync();
    }
}

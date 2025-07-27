using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;

namespace VenusBeauty.BLL.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> ObtenerClientesAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task<bool> CrearClienteAsync(Cliente cliente, string password);
        Task<bool> EditarClienteAsync(int id, Cliente cliente);
        Task<bool> EliminarClienteAsync(int id);
        Task<bool> CambiarEstadoAsync(int id);
    }
}

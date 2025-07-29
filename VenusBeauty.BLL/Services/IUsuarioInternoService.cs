using System.Collections.Generic;
using System.Threading.Tasks;

namespace VenusBeauty.BLL.Services
{
    public interface IUsuarioInternoService
    {
        Task<bool> CrearUsuarioInternoAsync(
            string email,
            string password,
            string rol,
            string nombre,
            string apellido,
            string telefono
        );

        Task<IEnumerable<UsuarioInternoDto>> ObtenerUsuariosInternosAsync();
        Task<UsuarioInternoDto> ObtenerPorIdAsync(string userId);

        Task<bool> EditarUsuarioInternoAsync(
            string userId,
            string email,
            string rol,
            string nombre,
            string apellido,
            string telefono);

        Task<bool> EliminarUsuarioInternoAsync(string userId);
    }
}

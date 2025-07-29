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
    }
}

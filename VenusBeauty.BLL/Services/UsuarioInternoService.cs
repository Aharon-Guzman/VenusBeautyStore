using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities; // si tu entidad Trabajadores aún no existe, la agregaremos después
using Microsoft.EntityFrameworkCore;

namespace VenusBeauty.BLL.Services
{
    public class UsuarioInternoService : IUsuarioInternoService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly VenusBeautyContext _context;

        public UsuarioInternoService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            VenusBeautyContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> CrearUsuarioInternoAsync(string email, string password, string rol, string nombre, string apellido, string telefono)
        {
            var usuario = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(usuario, password);

            if (!resultado.Succeeded)
                return false;

            if (!await _roleManager.RoleExistsAsync(rol))
                await _roleManager.CreateAsync(new IdentityRole(rol));

            await _userManager.AddToRoleAsync(usuario, rol);

            // Insertar en Trabajadores
            await _context.Database.ExecuteSqlRawAsync(@"
                insert into Trabajadores (UserId, Nombre, Apellido, Telefono, Rol)
                values ({0}, {1}, {2}, {3}, {4})",
                usuario.Id, nombre, apellido, telefono, rol);

            return true;
        }

        public async Task<IEnumerable<UsuarioInternoDto>> ObtenerUsuariosInternosAsync()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<UsuarioInternoDto>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);

                if (roles.Any(r =>
                    r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Recepcionista", StringComparison.OrdinalIgnoreCase) ||
                    r.Equals("Estilista", StringComparison.OrdinalIgnoreCase)))
                {
                    lista.Add(new UsuarioInternoDto
                    {
                        Email = usuario.Email,
                        Rol = string.Join(", ", roles)
                    });
                }
            }

            return lista;
        }
    }
}

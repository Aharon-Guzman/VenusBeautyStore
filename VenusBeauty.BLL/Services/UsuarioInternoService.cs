using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VenusBeauty.DAL.Context;
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

        public async Task<bool> CrearUsuarioInternoAsync(
            string email,
            string password,
            string rol,
            string nombre,
            string apellido,
            string telefono)
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

        public async Task<UsuarioInternoDto> ObtenerPorIdAsync(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return null;

            var trabajador = await _context.Trabajadores
                .Where(t => t.UserId == userId)
                .FirstOrDefaultAsync();

            if (trabajador == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario);

            return new UsuarioInternoDto
            {
                Email = usuario.Email,
                Rol = roles.FirstOrDefault() ?? ""
            };
        }

        public async Task<bool> EditarUsuarioInternoAsync(
            string userId,
            string email,
            string rol,
            string nombre,
            string apellido,
            string telefono)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            usuario.Email = email;
            usuario.UserName = email;
            var resultado = await _userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded) return false;

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, rol);

            var trabajador = await _context.Trabajadores.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trabajador != null)
            {
                trabajador.Nombre = nombre;
                trabajador.Apellido = apellido;
                trabajador.Telefono = telefono;
                trabajador.Rol = rol;
                _context.Trabajadores.Update(trabajador);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> EliminarUsuarioInternoAsync(string userId)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            var trabajador = await _context.Trabajadores.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trabajador != null)
            {
                _context.Trabajadores.Remove(trabajador);
                await _context.SaveChangesAsync();
            }

            var resultado = await _userManager.DeleteAsync(usuario);
            return resultado.Succeeded;
        }
    }
}

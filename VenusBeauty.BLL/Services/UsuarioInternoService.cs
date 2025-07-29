using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VenusBeauty.BLL.Services
{
    public class UsuarioInternoService : IUsuarioInternoService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
    

       public UsuarioInternoService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CrearUsuarioInternoAsync(string email, string password, string rol)
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

            return true;
        }
    }
}

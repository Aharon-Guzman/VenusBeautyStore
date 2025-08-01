using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.BLL.Services
{
    public class TrabajadorService : ITrabajadorService
    {
        private readonly ITrabajadorRepository _trabajadorRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrabajadorService(ITrabajadorRepository trabajadorRepository, UserManager<ApplicationUser> userManager)
        {
            _trabajadorRepository = trabajadorRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Trabajador>> ObtenerTrabajadoresAsync()
        {
            return await _trabajadorRepository.GetAllAsync();
        }

        public async Task<Trabajador?> ObtenerPorIdAsync(int id)
        {
            return await _trabajadorRepository.GetByIdAsync(id);
        }

        // ✅ Crear trabajador + usuario Identity + asignar rol
        public async Task<bool> CrearTrabajadorAsync(Trabajador trabajador, string password, string rol)
        {
            var user = new ApplicationUser
            {
                UserName = trabajador.Nombre.ToLower() + "." + trabajador.Apellido.ToLower(),
                Email = $"{trabajador.Nombre.ToLower()}.{trabajador.Apellido.ToLower()}@venusbeauty.com",
                EmailConfirmed = true,
                Nombres = trabajador.Nombre,
                Apellidos = trabajador.Apellido,
                DisplayName = $"{trabajador.Nombre} {trabajador.Apellido}",
                FotoUrl = "" // Opcional
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;

            await _userManager.AddToRoleAsync(user, rol);

            trabajador.UserId = user.Id;

            await _trabajadorRepository.AddAsync(trabajador);
            await _trabajadorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditarTrabajadorAsync(int id, Trabajador trabajador)
        {
            var trabajadorDb = await _trabajadorRepository.GetByIdAsync(id);
            if (trabajadorDb == null)
                return false;

            trabajadorDb.Nombre = trabajador.Nombre;
            trabajadorDb.Apellido = trabajador.Apellido;
            trabajadorDb.Telefono = trabajador.Telefono;
            trabajadorDb.Rol = trabajador.Rol;

            await _trabajadorRepository.UpdateAsync(trabajadorDb);
            await _trabajadorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarTrabajadorAsync(int id)
        {
            var trabajador = await _trabajadorRepository.GetByIdAsync(id);
            if (trabajador == null)
                return false;

            // 🔹 Primero eliminar el usuario Identity
            if (!string.IsNullOrEmpty(trabajador.UserId))
            {
                var user = await _userManager.FindByIdAsync(trabajador.UserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            // 🔹 Vuelve a consultar por si ya se eliminó en cascada
            var trabajadorRefrescado = await _trabajadorRepository.GetByIdAsync(id);
            if (trabajadorRefrescado == null)
                return true; // Ya no existe, no se hace nada

            await _trabajadorRepository.DeleteAsync(trabajadorRefrescado);
            await _trabajadorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CambiarPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // 🔹 Generamos token y cambiamos la contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }

    }
}

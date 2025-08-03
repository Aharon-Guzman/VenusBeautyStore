using Microsoft.AspNetCore.Identity;
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

        // ✅ Crear trabajador con correo digitado y errores detallados
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CrearTrabajadorAsync(
            Trabajador trabajador,
            string password,
            string rol,
            string email)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombres = trabajador.Nombre,
                Apellidos = trabajador.Apellido,
                DisplayName = $"{trabajador.Nombre} {trabajador.Apellido}",
                FotoUrl = "Valor por defecto no nulo"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, rol);

            trabajador.UserId = user.Id;

            await _trabajadorRepository.AddAsync(trabajador);
            await _trabajadorRepository.SaveChangesAsync();

            return (true, Enumerable.Empty<string>());
        }

        public async Task<bool> EditarTrabajadorAsync(int id, Trabajador trabajador)
        {
            var trabajadorDb = await _trabajadorRepository.GetByIdAsync(id);
            if (trabajadorDb == null)
                return false;

            // 🔹 Obtener usuario Identity
            var user = await _userManager.FindByIdAsync(trabajadorDb.UserId);
            if (user == null)
                return false;

            // ✅ Si el correo cambió → actualizar en Identity
            if (!string.Equals(user.Email, $"{trabajador.Nombre.ToLower()}.{trabajador.Apellido.ToLower()}@venusbeauty.com", StringComparison.OrdinalIgnoreCase))
            {
                var nuevoCorreo = $"{trabajador.Nombre.ToLower()}.{trabajador.Apellido.ToLower()}@venusbeauty.com";
                user.Email = nuevoCorreo;
                user.UserName = nuevoCorreo;

                var emailUpdate = await _userManager.UpdateAsync(user);
                if (!emailUpdate.Succeeded)
                    return false;
            }

            // ✅ Si el rol cambió → actualizar en Identity
            if (trabajadorDb.Rol != trabajador.Rol)
            {
                var rolesActuales = await _userManager.GetRolesAsync(user);
                if (rolesActuales.Any())
                    await _userManager.RemoveFromRolesAsync(user, rolesActuales);

                await _userManager.AddToRoleAsync(user, trabajador.Rol);
            }

            // ✅ Actualizamos nombre/apellido en Identity
            user.Nombres = trabajador.Nombre;
            user.Apellidos = trabajador.Apellido;
            user.DisplayName = $"{trabajador.Nombre} {trabajador.Apellido}";
            await _userManager.UpdateAsync(user);

            // ✅ Actualizamos la tabla Trabajadores
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

            if (!string.IsNullOrEmpty(trabajador.UserId))
            {
                var user = await _userManager.FindByIdAsync(trabajador.UserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            var trabajadorRefrescado = await _trabajadorRepository.GetByIdAsync(id);
            if (trabajadorRefrescado == null)
                return true;

            await _trabajadorRepository.DeleteAsync(trabajadorRefrescado);
            await _trabajadorRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CambiarPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }
        public async Task<(bool Succeeded, IEnumerable<string> Errors)> CrearTrabajadorConErroresAsync(
    Trabajador trabajador, string password, string rol, string email)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Nombres = trabajador.Nombre,
                Apellidos = trabajador.Apellido,
                DisplayName = $"{trabajador.Nombre} {trabajador.Apellido}",
                FotoUrl = "Valor por defecto no nulo"
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, rol);

            trabajador.UserId = user.Id;

            await _trabajadorRepository.AddAsync(trabajador);
            await _trabajadorRepository.SaveChangesAsync();

            return (true, Enumerable.Empty<string>());
        }
        public async Task<IEnumerable<Trabajador>> ObtenerEstilistasAsync()
        {
            return await _trabajadorRepository.GetEstilistasAsync();
        }
    }
}

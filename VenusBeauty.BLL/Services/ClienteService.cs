using Microsoft.AspNetCore.Identity;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeauty.BLL.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteService(IClienteRepository clienteRepository, UserManager<ApplicationUser> userManager)
        {
            _clienteRepository = clienteRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Cliente>> ObtenerClientesAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task<bool> CrearClienteAsync(Cliente cliente, string password)
        {
            var user = new ApplicationUser
            {
                UserName = cliente.Correo,
                Email = cliente.Correo,
                EmailConfirmed = true,
                Nombres = cliente.Nombre,
                Apellidos = $"{cliente.Apellido1} {cliente.Apellido2}",
                DisplayName = $"{cliente.Nombre} {cliente.Apellido1}",
                FotoUrl = "" // Puedes dejarlo vacío por ahora
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return false;

            cliente.UserId = user.Id;
            cliente.Activo = true;

            await _clienteRepository.AddAsync(cliente);
            await _clienteRepository.SaveChangesAsync();

            return true;
        }


        public async Task<bool> EditarClienteAsync(int id, Cliente cliente)
        {
            var clienteDb = await _clienteRepository.GetByIdAsync(id);
            if (clienteDb == null)
                return false;

            if (clienteDb.Correo != cliente.Correo)
            {
                var user = await _userManager.FindByIdAsync(clienteDb.UserId);
                if (user != null)
                {
                    user.Email = cliente.Correo;
                    user.UserName = cliente.Correo;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                        return false;
                }
            }

            clienteDb.Nombre = cliente.Nombre;
            clienteDb.Apellido1 = cliente.Apellido1;
            clienteDb.Apellido2 = cliente.Apellido2;
            clienteDb.Telefono = cliente.Telefono;
            clienteDb.Correo = cliente.Correo;
            clienteDb.Activo = cliente.Activo;

            await _clienteRepository.UpdateAsync(clienteDb);
            await _clienteRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return false;

            if (!string.IsNullOrEmpty(cliente.UserId))
            {
                var user = await _userManager.FindByIdAsync(cliente.UserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            var clienteRefrescado = await _clienteRepository.GetByIdAsync(id);
            if (clienteRefrescado == null)
                return true; // Si ya no existe, salimos sin error

            await _clienteRepository.DeleteAsync(clienteRefrescado);
            await _clienteRepository.SaveChangesAsync();

            return true;
        }


        public async Task<bool> CambiarEstadoAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return false;

            cliente.Activo = !cliente.Activo;

            await _clienteRepository.UpdateAsync(cliente);
            await _clienteRepository.SaveChangesAsync();

            return true;
        }
    }
}

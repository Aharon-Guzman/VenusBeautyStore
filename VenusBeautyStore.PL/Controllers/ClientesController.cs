//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using VenusBeauty.DAL.Entities;
//using VenusBeauty.DAL.Repositories; // ✅ Importamos la interfaz del repository

//namespace VenusBeautyStore.PL.Controllers
//{
//    [Authorize(Roles = "Admin,Recepcionista")]
//    public class ClientesController : Controller
//    {
//        private readonly IClienteRepository _clienteRepository;
//        private readonly UserManager<IdentityUser> _userManager;

//        public ClientesController(IClienteRepository clienteRepository, UserManager<IdentityUser> userManager)
//        {
//            _clienteRepository = clienteRepository;
//            _userManager = userManager;
//        }

//        // ✅ Listar clientes
//        public async Task<IActionResult> Index()
//        {
//            var clientes = await _clienteRepository.GetAllAsync();
//            return View(clientes);
//        }

//        // ✅ Mostrar formulario de creación
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // ✅ Crear cliente y usuario Identity
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Cliente cliente, string Password)
//        {
//            if (!ModelState.IsValid)
//                return View(cliente);

//            // 1️⃣ Crear usuario en Identity
//            var user = new IdentityUser
//            {
//                UserName = cliente.Correo,
//                Email = cliente.Correo,
//                EmailConfirmed = true
//            };

//            var result = await _userManager.CreateAsync(user, Password);

//            if (result.Succeeded)
//            {
//                // 2️⃣ Crear cliente en la base de datos
//                cliente.UserId = user.Id;
//                cliente.Activo = true;

//                await _clienteRepository.AddAsync(cliente);
//                await _clienteRepository.SaveChangesAsync();

//                return RedirectToAction(nameof(Index));
//            }

//            // 3️⃣ Si hay errores, mostrarlos
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error.Description);
//            }

//            return View(cliente);
//        }

//        // ✅ Mostrar formulario de edición
//        [HttpGet]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var cliente = await _clienteRepository.GetByIdAsync(id);
//            if (cliente == null)
//                return NotFound();

//            return View(cliente);
//        }

//        // ✅ Guardar cambios de edición
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Cliente cliente)
//        {
//            if (id != cliente.IdCliente)
//                return NotFound();

//            if (!ModelState.IsValid)
//                return View(cliente);

//            // 🔹 Obtener cliente actual
//            var clienteDb = await _clienteRepository.GetByIdAsync(id);
//            if (clienteDb == null)
//                return NotFound();

//            // 🔹 Si cambió el correo, actualizar también en Identity
//            if (clienteDb.Correo != cliente.Correo)
//            {
//                var user = await _userManager.FindByIdAsync(clienteDb.UserId);
//                if (user != null)
//                {
//                    user.Email = cliente.Correo;
//                    user.UserName = cliente.Correo;

//                    var result = await _userManager.UpdateAsync(user);
//                    if (!result.Succeeded)
//                    {
//                        foreach (var error in result.Errors)
//                            ModelState.AddModelError("", error.Description);

//                        return View(cliente); // Si falla Identity, no seguimos
//                    }
//                }
//            }

//            // 🔹 Actualizar datos en la tabla Clientes
//            clienteDb.Nombre = cliente.Nombre;
//            clienteDb.Apellido1 = cliente.Apellido1;
//            clienteDb.Apellido2 = cliente.Apellido2;
//            clienteDb.Telefono = cliente.Telefono;
//            clienteDb.Correo = cliente.Correo;
//            clienteDb.Activo = cliente.Activo;

//            await _clienteRepository.UpdateAsync(clienteDb);
//            await _clienteRepository.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }


//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var cliente = await _clienteRepository.GetByIdAsync(id);
//            if (cliente == null)
//                return NotFound();

//            return View(cliente); // ✅ Muestra la vista de confirmación
//        }


//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var cliente = await _clienteRepository.GetByIdAsync(id);
//            if (cliente == null)
//                return NotFound();

//            // 1️⃣ Eliminar usuario en Identity
//            if (!string.IsNullOrEmpty(cliente.UserId))
//            {
//                var user = await _userManager.FindByIdAsync(cliente.UserId);
//                if (user != null)
//                    await _userManager.DeleteAsync(user);
//            }

//            // 2️⃣ Eliminar cliente en base de datos
//            await _clienteRepository.DeleteAsync(cliente); // ✅ Pasamos el objeto
//            await _clienteRepository.SaveChangesAsync();

//            return RedirectToAction(nameof(Index));
//        }



//    }
//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.DAL.Entities;
using VenusBeauty.DAL.Repositories;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class ClientesController : Controller
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientesController(IClienteRepository clienteRepository, UserManager<IdentityUser> userManager)
        {
            _clienteRepository = clienteRepository;
            _userManager = userManager;
        }

        // ✅ Listar clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return View(clientes);
        }

        // ✅ Mostrar formulario de creación
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Crear cliente y usuario Identity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente, string Password)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            var user = new IdentityUser
            {
                UserName = cliente.Correo,
                Email = cliente.Correo,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                cliente.UserId = user.Id;
                cliente.Activo = true;

                await _clienteRepository.AddAsync(cliente);
                await _clienteRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(cliente);
        }

        // ✅ Mostrar formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ✅ Guardar cambios de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return NotFound();

            if (!ModelState.IsValid)
                return View(cliente);

            var clienteDb = await _clienteRepository.GetByIdAsync(id);
            if (clienteDb == null)
                return NotFound();

            // 🔹 Si cambió el correo, actualizar también en Identity
            if (clienteDb.Correo != cliente.Correo)
            {
                var user = await _userManager.FindByIdAsync(clienteDb.UserId);
                if (user != null)
                {
                    user.Email = cliente.Correo;
                    user.UserName = cliente.Correo;

                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error.Description);

                        return View(cliente);
                    }
                }
            }

            // 🔹 Actualizar datos
            clienteDb.Nombre = cliente.Nombre;
            clienteDb.Apellido1 = cliente.Apellido1;
            clienteDb.Apellido2 = cliente.Apellido2;
            clienteDb.Telefono = cliente.Telefono;
            clienteDb.Correo = cliente.Correo;
            clienteDb.Activo = cliente.Activo;

            await _clienteRepository.UpdateAsync(clienteDb);
            await _clienteRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ✅ Mostrar confirmación de eliminación
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // ✅ Confirmar y eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            // 1️⃣ Eliminar usuario de Identity
            if (!string.IsNullOrEmpty(cliente.UserId))
            {
                var user = await _userManager.FindByIdAsync(cliente.UserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            // 2️⃣ Volver a obtener el cliente antes de eliminar (evita error de concurrencia)
            var clienteParaEliminar = await _clienteRepository.GetByIdAsync(id);
            if (clienteParaEliminar != null)
            {
                await _clienteRepository.DeleteAsync(clienteParaEliminar);
                await _clienteRepository.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

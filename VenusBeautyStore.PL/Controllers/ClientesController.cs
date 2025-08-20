
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        //  Listar clientes
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteService.ObtenerClientesAsync();
            return View(clientes);
        }

        //  Mostrar formulario de creación
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //  Crear cliente y usuario Identity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente, string Password)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            var creado = await _clienteService.CrearClienteAsync(cliente, Password);

            if (creado)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al crear el cliente");
            return View(cliente);
        }

        //  Mostrar formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        //  Guardar cambios de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(cliente);

            var actualizado = await _clienteService.EditarClienteAsync(id, cliente);

            if (actualizado)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Error al actualizar el cliente");
            return View(cliente);
        }

        //  Mostrar confirmación de eliminación
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        //  Confirmar y eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clienteService.EliminarClienteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //  Cambiar estado (botón slide con AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo([FromForm] int id)
        {
            var cambiado = await _clienteService.CambiarEstadoAsync(id);
            if (!cambiado)
                return NotFound();

            return Ok(new { success = true });
        }
    }
}

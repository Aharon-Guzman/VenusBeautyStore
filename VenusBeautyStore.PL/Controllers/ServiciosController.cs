using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class ServiciosController : Controller
    {
        private readonly IServicioService _servicioService;

        public ServiciosController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        // ✅ Listar servicios
        public async Task<IActionResult> Index()
        {
            var servicios = await _servicioService.ObtenerServiciosAsync();
            return View(servicios);
        }

        // ✅ Mostrar formulario de creación
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Crear servicio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servicio servicio)
        {
            if (!ModelState.IsValid)
                return View(servicio);

            await _servicioService.CrearServicioAsync(servicio);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Mostrar formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var servicio = await _servicioService.ObtenerPorIdAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // ✅ Guardar cambios de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Servicio servicio)
        {
            if (id != servicio.IdServicio)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(servicio);

            var actualizado = await _servicioService.EditarServicioAsync(id, servicio);
            if (!actualizado)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // ✅ Mostrar confirmación de eliminación
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var servicio = await _servicioService.ObtenerPorIdAsync(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        // ✅ Confirmar y eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _servicioService.EliminarServicioAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Cambiar estado con AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo([FromForm] int id)
        {
            var cambiado = await _servicioService.CambiarEstadoAsync(id);
            if (!cambiado)
                return NotFound();

            return Ok(new { success = true });
        }
    }
}

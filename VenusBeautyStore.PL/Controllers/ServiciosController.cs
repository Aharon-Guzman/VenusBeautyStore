//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using VenusBeauty.BLL.Services;
//using VenusBeauty.DAL.Entities;

//namespace VenusBeautyStore.PL.Controllers
//{
//    [Authorize(Roles = "Admin,Recepcionista")]
//    public class ServiciosController : Controller
//    {
//        private readonly IServicioService _servicioService;

//        public ServiciosController(IServicioService servicioService)
//        {
//            _servicioService = servicioService;
//        }

//        // ✅ Listar servicios
//        public async Task<IActionResult> Index()
//        {
//            var servicios = await _servicioService.ObtenerServiciosAsync();
//            return View(servicios);
//        }

//        // ✅ Mostrar formulario de creación
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // ✅ Crear servicio
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Servicio servicio)
//        {
//            if (!ModelState.IsValid)
//                return View(servicio);

//            await _servicioService.CrearServicioAsync(servicio);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Mostrar formulario de edición
//        [HttpGet]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var servicio = await _servicioService.ObtenerPorIdAsync(id);
//            if (servicio == null)
//                return NotFound();

//            return View(servicio);
//        }

//        // ✅ Guardar cambios de edición
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Servicio servicio)
//        {
//            if (id != servicio.IdServicio)
//                return BadRequest();

//            if (!ModelState.IsValid)
//                return View(servicio);

//            var actualizado = await _servicioService.EditarServicioAsync(id, servicio);
//            if (!actualizado)
//                return NotFound();

//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Mostrar confirmación de eliminación
//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var servicio = await _servicioService.ObtenerPorIdAsync(id);
//            if (servicio == null)
//                return NotFound();

//            return View(servicio);
//        }

//        // ✅ Confirmar y eliminar
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _servicioService.EliminarServicioAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Cambiar estado con AJAX
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ToggleActivo([FromForm] int id)
//        {
//            var cambiado = await _servicioService.CambiarEstadoAsync(id);
//            if (!cambiado)
//                return NotFound();

//            return Ok(new { success = true });
//        }
//    }
//}

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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ServiciosController(IServicioService servicioService, IWebHostEnvironment webHostEnvironment)
        {
            _servicioService = servicioService;
            _webHostEnvironment = webHostEnvironment;
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

        // ✅ Crear servicio con imagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Servicio servicio, IFormFile? ImagenArchivo)
        {
            if (!ModelState.IsValid)
                return View(servicio);

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                string uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "servicios");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string fileName = Guid.NewGuid() + Path.GetExtension(ImagenArchivo.FileName);
                string filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenArchivo.CopyToAsync(stream);
                }

                servicio.ImagenUrl = $"/uploads/servicios/{fileName}";
            }

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

        // ✅ Guardar cambios de edición con imagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Servicio servicio, IFormFile? ImagenArchivo)
        {
            if (id != servicio.IdServicio)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(servicio);

            var servDb = await _servicioService.ObtenerPorIdAsync(id);
            if (servDb == null)
                return NotFound();

            // 🔹 Si se subió una nueva imagen → eliminar la anterior y guardar la nueva
            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                // 🗑️ Eliminar la imagen anterior si existe
                if (!string.IsNullOrEmpty(servDb.ImagenUrl))
                {
                    string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, servDb.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // 📂 Guardar nueva imagen
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/servicios");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImagenArchivo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await ImagenArchivo.CopyToAsync(fileStream);

                servicio.ImagenUrl = $"/uploads/servicios/{uniqueFileName}";
            }
            else
            {
                // Mantener la imagen anterior si no se subió una nueva
                servicio.ImagenUrl = servDb.ImagenUrl;
            }

            await _servicioService.EditarServicioAsync(id, servicio);
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

        // ✅ Confirmar y eliminar servicio (y su imagen)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servDb = await _servicioService.ObtenerPorIdAsync(id);
            if (servDb == null)
                return NotFound();

            // 🗑️ Eliminar la imagen asociada si existe
            if (!string.IsNullOrEmpty(servDb.ImagenUrl))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, servDb.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            await _servicioService.EliminarServicioAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Cambiar estado (botón slide con AJAX)
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

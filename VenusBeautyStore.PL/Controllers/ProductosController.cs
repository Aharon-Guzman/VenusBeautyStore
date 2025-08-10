using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(IProductoService productoService, IWebHostEnvironment webHostEnvironment)
        {
            _productoService = productoService;
            _webHostEnvironment = webHostEnvironment;
        }

        // ✅ Listar productos
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerProductosAsync();
            return View(productos);
        }

        // ✅ Mostrar formulario de creación
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ Crear producto con imagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile? ImagenArchivo)
        {
            if (!ModelState.IsValid)
                return View(producto);

            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                string uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "productos");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                string fileName = Guid.NewGuid() + Path.GetExtension(ImagenArchivo.FileName);
                string filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenArchivo.CopyToAsync(stream);
                }

                producto.ImagenUrl = $"/uploads/productos/{fileName}";
            }

            await _productoService.CrearProductoAsync(producto);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Mostrar formulario de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto, IFormFile? ImagenArchivo)
        {
            if (id != producto.IdProducto)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(producto);

            var prodDb = await _productoService.ObtenerPorIdAsync(id);
            if (prodDb == null)
                return NotFound();

            // 🔹 Si se subió una nueva imagen → eliminar la anterior y guardar la nueva
            if (ImagenArchivo != null && ImagenArchivo.Length > 0)
            {
                // 🗑️ Eliminar la imagen anterior si existe
                if (!string.IsNullOrEmpty(prodDb.ImagenUrl))
                {
                    string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, prodDb.ImagenUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // 📂 Guardar nueva imagen
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/productos");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImagenArchivo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await ImagenArchivo.CopyToAsync(fileStream);

                producto.ImagenUrl = $"/uploads/productos/{uniqueFileName}";
            }
            else
            {
                // Mantener la imagen anterior si no se subió una nueva
                producto.ImagenUrl = prodDb.ImagenUrl;
            }

            await _productoService.EditarProductoAsync(id, producto);
            return RedirectToAction(nameof(Index));
        }

        // ✅ Mostrar confirmación de eliminación
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _productoService.ObtenerPorIdAsync(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prodDb = await _productoService.ObtenerPorIdAsync(id);
            if (prodDb == null)
                return NotFound();

            var eliminado = await _productoService.EliminarProductoAsync(id);
            if (!eliminado)
            {
                TempData["Error"] = "No se puede eliminar este producto porque está asociado a una o más citas.";
                return RedirectToAction(nameof(Index));
            }

            // 🗑️ Eliminar imagen física si existe
            if (!string.IsNullOrEmpty(prodDb.ImagenUrl))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, prodDb.ImagenUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            TempData["Success"] = "El producto fue eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ✅ Cambiar estado (botón slide con AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo([FromForm] int id)
        {
            var cambiado = await _productoService.CambiarEstadoAsync(id);
            if (!cambiado)
                return NotFound();

            return Ok(new { success = true });
        }
    }
}

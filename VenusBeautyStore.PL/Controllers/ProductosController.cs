//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using VenusBeauty.BLL.Services;
//using VenusBeauty.DAL.Entities;

//namespace VenusBeautyStore.PL.Controllers
//{
//    [Authorize(Roles = "Admin,Recepcionista")]
//    public class ProductosController : Controller
//    {
//        private readonly IProductoService _productoService;

//        public ProductosController(IProductoService productoService)
//        {
//            _productoService = productoService;
//        }

//        // ✅ Listar productos
//        public async Task<IActionResult> Index()
//        {
//            var productos = await _productoService.ObtenerProductosAsync();
//            return View(productos);
//        }

//        // ✅ Mostrar formulario de creación
//        [HttpGet]
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // ✅ Crear producto
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Producto producto)
//        {
//            if (!ModelState.IsValid)
//                return View(producto);

//            await _productoService.CrearProductoAsync(producto);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Mostrar formulario de edición
//        [HttpGet]
//        public async Task<IActionResult> Edit(int id)
//        {
//            var producto = await _productoService.ObtenerPorIdAsync(id);
//            if (producto == null)
//                return NotFound();

//            return View(producto);
//        }

//        // ✅ Guardar cambios de edición
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, Producto producto)
//        {
//            if (id != producto.IdProducto)
//                return BadRequest();

//            if (!ModelState.IsValid)
//                return View(producto);

//            var actualizado = await _productoService.EditarProductoAsync(id, producto);
//            if (!actualizado)
//                return NotFound();

//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Mostrar confirmación de eliminación
//        [HttpGet]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var producto = await _productoService.ObtenerPorIdAsync(id);
//            if (producto == null)
//                return NotFound();

//            return View(producto);
//        }

//        // ✅ Confirmar y eliminar
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            await _productoService.EliminarProductoAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        // ✅ Cambiar estado (botón slide con AJAX)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ToggleActivo([FromForm] int id)
//        {
//            var cambiado = await _productoService.CambiarEstadoAsync(id);
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

        // ✅ Guardar cambios de edición (con opción de cambiar imagen)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto, IFormFile? ImagenArchivo)
        {
            if (id != producto.IdProducto)
                return BadRequest();

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

            var actualizado = await _productoService.EditarProductoAsync(id, producto);
            if (!actualizado)
                return NotFound();

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

        // ✅ Confirmar y eliminar
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productoService.EliminarProductoAsync(id);
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

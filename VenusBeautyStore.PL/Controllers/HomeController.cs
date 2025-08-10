//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using VenusBeautyStore.PL.Models;

//namespace VenusBeautyStore.PL.Controllers
//{
//    [AllowAnonymous]
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        public HomeController(ILogger<HomeController> logger)
//        {
//            _logger = logger;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeautyStore.PL.Models;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductoService _productoService;

        public HomeController(ILogger<HomeController> logger, IProductoService productoService)
        {
            _logger = logger;
            _productoService = productoService;
        }

        // Home con “Productos destacados”
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerProductosAsync();

            var destacados = productos
                .Where(p => p.Activo && !string.IsNullOrWhiteSpace(p.ImagenUrl))
                .OrderByDescending(p => p.IdProducto)
                .Take(6)
                .ToList();

            // La vista Index recibirá IEnumerable<Producto>
            return View(destacados);
        }

        // (Para después) Página con todos los productos activos
        [HttpGet]
        public async Task<IActionResult> Productos()
        {
            var productos = await _productoService.ObtenerProductosAsync();
            var activos = productos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.IdProducto)
                .ToList();

            return View(activos); // haremos la vista luego
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

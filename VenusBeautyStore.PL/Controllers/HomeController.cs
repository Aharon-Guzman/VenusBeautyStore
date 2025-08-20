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
        private readonly IServicioService _servicioService;

        public HomeController(ILogger<HomeController> logger, IProductoService productoService, IServicioService servicioService)
        {
            _logger = logger;
            _productoService = productoService;
            _servicioService = servicioService;
        }

        // Home con “Productos destacados”
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new HomeIndexVM
            {
                Productos = (await _productoService.ObtenerProductosAsync())
                          .Where(p => p.Activo).Take(9).ToList(),
                Servicios = (await _servicioService.ObtenerServiciosAsync())
                          .Take(6).ToList()
            };


            // Si es interno (admin/recep/estilista), mándalo al portal de gestión
            if (User?.Identity?.IsAuthenticated == true &&
                (User.IsInRole("Admin") || User.IsInRole("Recepcionista") || User.IsInRole("Estilista")))
            {
                return RedirectToAction("Index", "Portal");
            }




            return View(vm);
        }

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

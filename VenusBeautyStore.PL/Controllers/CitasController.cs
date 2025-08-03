//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using VenusBeauty.DAL.Entities;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using VenusBeauty.BLL.Services;          // ICitaService, IClienteService, …
//using VenusBeautyStore.PL.Models;       // CitaCreateViewModel

//namespace VenusBeautyStore.PL.Controllers
//{
//    /*  
//       Dejamos solo [Authorize] para que Clientes también puedan crear citas.
//       Si quieres más control pon: Roles = "Admin,Recepcionista,Estilista,Cliente"
//    */
//    [Authorize("Roles = Admin, Recepcionista, Cliente")]
//    public class CitasController : Controller
//    {
//        private readonly ICitaService _citaService;
//        private readonly IClienteService _clienteService;
//        private readonly ITrabajadorService _trabajadorService;
//        private readonly IServicioService _servicioService;
//        private readonly IProductoService _productoService;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public CitasController(
//            ICitaService citaService,
//            IClienteService clienteService,
//            ITrabajadorService trabajadorService,
//            IServicioService servicioService,
//            IProductoService productoService,
//            UserManager<ApplicationUser> userManager)
//        {
//            _citaService = citaService;
//            _clienteService = clienteService;
//            _trabajadorService = trabajadorService;
//            _servicioService = servicioService;
//            _productoService = productoService;
//            _userManager = userManager;
//        }

//        /* ---------- LISTADO ---------- */
//        // GET: /Citas
//        public async Task<IActionResult> Index()
//        {
//            var citas = await _citaService.ObtenerAgendaAsync(
//                DateTime.Today.AddDays(-30),
//                DateTime.Today.AddDays(30));


//            return View(citas);
//        }

//        /* ---------- FORMULARIO CREATE (GET) ---------- */
//        // GET: /Citas/Create
//        // GET: /Citas/Create
//        public async Task<IActionResult> Create()
//        {
//            var vm = new CitaCreateViewModel
//            {
//                /* Estilistas, Servicios y Productos */
//                Trabajadores = (await _trabajadorService.ObtenerTrabajadoresAsync())
//                    .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId)),

//                Servicios = (await _servicioService.ObtenerServiciosAsync())
//                    .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString())),

//                Productos = (await _productoService.ObtenerProductosAsync())
//                    .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()))
//            };

//            if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
//            {
//                // Dropdown de clientes para personal interno
//                vm.Clientes = (await _clienteService.ObtenerClientesAsync())
//           .Select(c => new SelectListItem(
//               $"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),  // texto visible
//               c.IdCliente.ToString()));                           // value
//            }
//            else   // Rol Cliente
//            {
//                var userId = _userManager.GetUserId(User)!;

//                var cliente = (await _clienteService.ObtenerClientesAsync())
//                              .FirstOrDefault(c => c.UserId == userId);

//                if (cliente is null) return Forbid();

//                vm.IdCliente = cliente.IdCliente;   // pre-asignado (campo oculto)
//            }

//            vm.FechaHora = DateTime.Now.AddHours(1);
//            return View(vm);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CitaCreateViewModel vm)
//        {
//            /* ► VALIDACIÓN DE MODELO */
//            if (vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any())
//                ModelState.AddModelError(nameof(vm.ServiciosSeleccionados),
//                                         "Seleccione al menos un servicio.");

//            if (!ModelState.IsValid)
//            {
//                // ❶: Si algo falla, recargamos las listas para volver a mostrar la vista.
//                vm.Trabajadores = (await _trabajadorService.ObtenerTrabajadoresAsync())
//                    .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId));

//                vm.Servicios = (await _servicioService.ObtenerServiciosAsync())
//                    .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString()));

//                vm.Productos = (await _productoService.ObtenerProductosAsync())
//                    .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()));

//                if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
//                {
//                    vm.Clientes = (await _clienteService.ObtenerClientesAsync())
//                        .Select(c => new SelectListItem($"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),
//                                                        c.IdCliente.ToString()));
//                }

//                return View(vm);
//            }

//            /* ► CONSTRUIR diccionario limpio de productos (solo marcados y >0) */
//            var productosFiltrados = vm.ProductosSeleccionados?
//                .Where(kvp => kvp.Value > 0)
//                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

//            /* ► LLAMADA AL SERVICIO DE NEGOCIO */
//            await _citaService.CrearCitaAsync(
//                vm.IdCliente,
//                vm.IdUsuario,
//                vm.FechaHora,
//                vm.ServiciosSeleccionados,
//                productosFiltrados);

//            /* ► TODO OK → volvemos al listado */
//            TempData["Success"] = "Cita registrada correctamente.";
//            return RedirectToAction(nameof(Index));
//        }


//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;
using VenusBeautyStore.PL.Models;

namespace VenusBeautyStore.PL.Controllers
{
    /*  Deja entrar a todos los roles que crean o ven citas                */
    [Authorize]
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly IClienteService _clienteService;
        private readonly ITrabajadorService _trabajadorService;
        private readonly IServicioService _servicioService;
        private readonly IProductoService _productoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitasController(
            ICitaService citaService,
            IClienteService clienteService,
            ITrabajadorService trabajadorService,
            IServicioService servicioService,
            IProductoService productoService,
            UserManager<ApplicationUser> userManager)
        {
            _citaService = citaService;
            _clienteService = clienteService;
            _trabajadorService = trabajadorService;
            _servicioService = servicioService;
            _productoService = productoService;
            _userManager = userManager;
        }

        /* ---------- LISTADO ---------- */
        // GET: /Citas
        public async Task<IActionResult> Index()
        {
            DateTime desde = DateTime.Today.AddDays(-30);
            DateTime hasta = DateTime.Today.AddDays(30);
            IEnumerable<Cita> citas;

            /* Admin o Recepcionista → todas las citas */
            if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
            {
                citas = await _citaService.ObtenerAgendaAsync(desde, hasta);
            }
            /* Estilista → solo las que él atiende */
            else if (User.IsInRole("Estilista"))
            {
                var userId = _userManager.GetUserId(User)!;
                citas = await _citaService.ObtenerAgendaAsync(desde, hasta, userId);
            }
            /* Cliente → solo sus propias citas */
            else  // Rol Cliente
            {
                var userId = _userManager.GetUserId(User)!;
                var cliente = (await _clienteService.ObtenerClientesAsync())
                              .FirstOrDefault(c => c.UserId == userId);

                if (cliente is null) return Forbid();

                citas = (await _citaService.ObtenerAgendaAsync(desde, hasta))
                        .Where(c => c.IdCliente == cliente.IdCliente);
            }

            return View(citas);
        }

        /* ---------- FORMULARIO CREATE (GET) ---------- */
        // GET: /Citas/Create
        public async Task<IActionResult> Create()
        {
            var vm = new CitaCreateViewModel
            {
                Trabajadores = (await _trabajadorService.ObtenerTrabajadoresAsync())
                    .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId)),

                Servicios = (await _servicioService.ObtenerServiciosAsync())
                    .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString())),

                Productos = (await _productoService.ObtenerProductosAsync())
                    .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()))
            };

            if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
            {
                vm.Clientes = (await _clienteService.ObtenerClientesAsync())
                    .Select(c => new SelectListItem(
                        $"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),
                        c.IdCliente.ToString()));
            }
            else // Rol Cliente
            {
                var userId = _userManager.GetUserId(User)!;
                var cliente = (await _clienteService.ObtenerClientesAsync())
                              .FirstOrDefault(c => c.UserId == userId);

                if (cliente is null) return Forbid();

                vm.IdCliente = cliente.IdCliente;
            }

            //vm.FechaHora = DateTime.Now.AddHours(1);
            var ahora = DateTime.Now.AddHours(1);
            vm.FechaHora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0);
            return View(vm);
        }

        /* ---------- FORMULARIO CREATE (POST) ---------- */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaCreateViewModel vm)
        {
            /* Validación: al menos un servicio seleccionado */
            if (vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any())
                ModelState.AddModelError(nameof(vm.ServiciosSeleccionados),
                                         "Seleccione al menos un servicio.");
            if (vm.FechaHora < DateTime.Now)
            {
                ModelState.AddModelError(nameof(vm.FechaHora),
                    "No puede reservar una cita en el pasado.");
            }
            if (!ModelState.IsValid)
            {
                /* Recarga listas para redisplay */
                vm.Trabajadores = (await _trabajadorService.ObtenerTrabajadoresAsync())
                    .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId));

                vm.Servicios = (await _servicioService.ObtenerServiciosAsync())
                    .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString()));

                vm.Productos = (await _productoService.ObtenerProductosAsync())
                    .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()));

                if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
                {
                    vm.Clientes = (await _clienteService.ObtenerClientesAsync())
                        .Select(c => new SelectListItem($"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),
                                                        c.IdCliente.ToString()));
                }

                return View(vm);
            }



            /* Productos marcados y cantidad > 0 */
            var productosFiltrados = vm.ProductosSeleccionados?
                .Where(kvp => kvp.Value > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            /* Guarda la cita */
            await _citaService.CrearCitaAsync(
                vm.IdCliente,
                vm.IdUsuario,
                vm.FechaHora,
                vm.ServiciosSeleccionados,
                productosFiltrados);

            TempData["Success"] = "Cita registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}

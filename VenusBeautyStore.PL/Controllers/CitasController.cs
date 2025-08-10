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
                Trabajadores = (await _trabajadorService.ObtenerEstilistasActivosAsync())
                     .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId)),


                Servicios = (await _servicioService.ObtenerServiciosActivosAsync())
                     .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString())),


                Productos = (await _productoService.ObtenerProductosActivosAsync())
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
            // Validación: al menos un servicio seleccionado
            if (vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any())
            {
                ModelState.AddModelError(nameof(vm.ServiciosSeleccionados),
                                         "Seleccione al menos un servicio.");
            }

            // Validar que no sea en el pasado
            if (vm.FechaHora < DateTime.Now)
            {
                ModelState.AddModelError(nameof(vm.FechaHora),
                                         "No puede reservar una cita en el pasado.");
            }

            if (!ModelState.IsValid)
            {
                await CargarListas(vm);
                return View(vm);
            }

            try
            {
                var productosFiltrados = vm.ProductosSeleccionados?
                    .Where(kvp => kvp.Value > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                await _citaService.CrearCitaAsync(
                    vm.IdCliente,
                    vm.IdUsuario,
                    vm.FechaHora,
                    vm.ServiciosSeleccionados,
                    productosFiltrados);

                TempData["Success"] = "Cita registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // ⚠️ Si hay solapamiento u otro error de negocio
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListas(vm);
                return View(vm);
            }
        }

        private async Task CargarListas(CitaCreateViewModel vm)
        {
            vm.Trabajadores = (await _trabajadorService.ObtenerTrabajadoresAsync())
                .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId));

            vm.Servicios = (await _servicioService.ObtenerServiciosAsync())
                .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString()));

            vm.Productos = (await _productoService.ObtenerProductosAsync())
                .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()));

            if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
            {
                vm.Clientes = (await _clienteService.ObtenerClientesAsync())
                    .Select(c => new SelectListItem(
                        $"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),
                        c.IdCliente.ToString()));
            }
        }
        // GET: /Citas/Edit/5
        [Authorize(Roles = "Admin,Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cita = await _citaService.ObtenerCitaAsync(id);
            if (cita is null) return NotFound();

            var vm = new CitaEditEstadoViewModel
            {
                IdCita = cita.IdCita,
                Estado = cita.Estado
            };

            return View(vm);
        }
        // POST: /Citas/Edit
        [Authorize(Roles = "Admin,Recepcionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VenusBeautyStore.PL.Models.CitaEditEstadoViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var ok = await _citaService.ActualizarEstadoAsync(vm.IdCita, vm.Estado);
                if (!ok) return NotFound();

                TempData["Success"] = "Estado actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
        }
        // GET: /Citas/Delete/5
        [Authorize(Roles = "Admin,Recepcionista")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var cita = await _citaService.ObtenerCitaAsync(id);
            if (cita is null) return NotFound();

            var vm = new CitaDeleteViewModel
            {
                IdCita = cita.IdCita,
                Cliente = cita.Cliente != null
                            ? $"{cita.Cliente.Nombre} {cita.Cliente.Apellido1} {cita.Cliente.Apellido2}".Trim()
                            : $"Cliente #{cita.IdCliente}",
                Estilista = cita.Trabajador != null
                            ? $"{cita.Trabajador.Nombre} {cita.Trabajador.Apellido}".Trim()
                            : "(sin estilista)",
                FechaHora = cita.FechaHora,
                Estado = cita.Estado
            };

            return View(vm);
        }
        [Authorize(Roles = "Admin,Recepcionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VenusBeautyStore.PL.Models.CitaDeleteViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var ok = await _citaService.EliminarCitaAsync(vm.IdCita);
                if (!ok) return NotFound();

                TempData["Success"] = "Cita eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Regla de negocio (p.ej. no eliminar Completada)
                ModelState.AddModelError(string.Empty, ex.Message);

                // Recargar datos de la cita para volver a mostrar el resumen en la vista
                var cita = await _citaService.ObtenerCitaAsync(vm.IdCita);
                if (cita != null)
                {
                    vm.Cliente = cita.Cliente != null ? $"{cita.Cliente.Nombre} {cita.Cliente.Apellido1} {cita.Cliente.Apellido2}".Trim() : $"Cliente #{cita.IdCliente}";
                    vm.Estilista = cita.Trabajador != null ? $"{cita.Trabajador.Nombre} {cita.Trabajador.Apellido}".Trim() : "(sin estilista)";
                    vm.FechaHora = cita.FechaHora;
                    vm.Estado = cita.Estado;
                }

                return View(vm);
            }
        }


    }
}

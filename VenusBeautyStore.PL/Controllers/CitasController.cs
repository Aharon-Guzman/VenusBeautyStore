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

        // GET: /Citas/Create
        //public async Task<IActionResult> Create()
        //{
        //    var vm = new CitaCreateViewModel
        //    {
        //        Trabajadores = (await _trabajadorService.ObtenerEstilistasActivosAsync())
        //             .Select(t => new SelectListItem($"{t.Nombre} {t.Apellido}", t.UserId)),


        //        Servicios = (await _servicioService.ObtenerServiciosActivosAsync())
        //             .Select(s => new SelectListItem(s.Nombre, s.IdServicio.ToString())),


        //        Productos = (await _productoService.ObtenerProductosActivosAsync())
        //            .Select(p => new SelectListItem(p.Nombre, p.IdProducto.ToString()))

        //    };

        //    if (User.IsInRole("Admin") || User.IsInRole("Recepcionista"))
        //    {
        //        vm.Clientes = (await _clienteService.ObtenerClientesAsync())
        //            .Select(c => new SelectListItem(
        //                $"{c.Nombre} {c.Apellido1} {c.Apellido2}".Trim(),
        //                c.IdCliente.ToString()));
        //    }
        //    else // Rol Cliente
        //    {
        //        var userId = _userManager.GetUserId(User)!;
        //        var cliente = (await _clienteService.ObtenerClientesAsync())
        //                      .FirstOrDefault(c => c.UserId == userId);

        //        if (cliente is null) return Forbid();

        //        vm.IdCliente = cliente.IdCliente;
        //    }

        //    //vm.FechaHora = DateTime.Now.AddHours(1);
        //    var ahora = DateTime.Now.AddHours(1);
        //    vm.FechaHora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0);
        //    return View(vm);
        //}

        public async Task<IActionResult> Create(int? idServicio = null, int? idProducto = null, int cantidad = 1)
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

            var ahora = DateTime.Now.AddHours(1);
            vm.FechaHora = new DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, ahora.Minute, 0);

            // 👉 Guardamos los “pendientes” para usarlos en la vista y luego en el POST
            ViewBag.IdServicioPendiente = idServicio;
            ViewBag.IdProductoPendiente = idProducto;
            ViewBag.CantidadPendiente = cantidad;

            // 👉 (opcional) marcar el servicio pendiente en los checkboxes
            if (idServicio.HasValue)
                vm.ServiciosSeleccionados = new List<int> { idServicio.Value };
            if (idProducto.HasValue)
                vm.ProductosSeleccionados[idProducto.Value] = Math.Max(1, cantidad);
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CitaCreateViewModel vm)
        {
            // 🔸 Leer pendientes que vienen como hidden del formulario
            int? idServPend = null;
            int? idProdPend = null;
            int cantPend = 1;
            if (int.TryParse(Request.Form["idServicioPendiente"], out var sId)) idServPend = sId;
            if (int.TryParse(Request.Form["idProductoPendiente"], out var pId)) idProdPend = pId;
            if (int.TryParse(Request.Form["cantidadPendiente"], out var qty)) cantPend = Math.Max(1, qty);

            // 🔸 Si NO hay servicios marcados pero SÍ hay servicio pendiente, lo agregamos aquí
            if ((vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any()) && idServPend.HasValue)
            {
                vm.ServiciosSeleccionados = new List<int> { idServPend.Value };
            }

            // ❗ Si sigues sin ningún servicio (y solo hay producto pendiente), bloquear (tu BLL exige al menos un servicio)
            if (vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any())
            {
                ModelState.AddModelError(nameof(vm.ServiciosSeleccionados), "Seleccione al menos un servicio.");
            }

            // Validación de fecha (como ya tenías)
            if (vm.FechaHora < DateTime.Now)
            {
                ModelState.AddModelError(nameof(vm.FechaHora), "No puede reservar una cita en el pasado.");
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

                // ✅ Crear cita (ya trae el servicio pendiente si existía)
                var nuevaCitaId = await _citaService.CrearCitaAsync(
                    vm.IdCliente,
                    vm.IdUsuario,
                    vm.FechaHora,
                    vm.ServiciosSeleccionados,
                    productosFiltrados);

                // Si además había PRODUCTO pendiente, ahora lo añadimos y vamos a MiCarrito
                if (idProdPend.HasValue)
                {
                    var userId = _userManager.GetUserId(User)!;
                    await _citaService.AgregarProductoAsync(nuevaCitaId, idProdPend.Value, cantPend, userId);
                }

                // Ir al carrito de la nueva cita
                return RedirectToAction(nameof(MiCarrito), new { id = nuevaCitaId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarListas(vm);
                return View(vm);
            }
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CitaCreateViewModel vm)
        //{
        //    // Validación: al menos un servicio seleccionado
        //    if (vm.ServiciosSeleccionados == null || !vm.ServiciosSeleccionados.Any())
        //    {
        //        ModelState.AddModelError(nameof(vm.ServiciosSeleccionados),
        //                                 "Seleccione al menos un servicio.");
        //    }

        //    // Validar que no sea en el pasado
        //    if (vm.FechaHora < DateTime.Now)
        //    {
        //        ModelState.AddModelError(nameof(vm.FechaHora),
        //                                 "No puede reservar una cita en el pasado.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        await CargarListas(vm);
        //        return View(vm);
        //    }

        //    try
        //    {
        //        var productosFiltrados = vm.ProductosSeleccionados?
        //            .Where(kvp => kvp.Value > 0)
        //            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        //        await _citaService.CrearCitaAsync(
        //            vm.IdCliente,
        //            vm.IdUsuario,
        //            vm.FechaHora,
        //            vm.ServiciosSeleccionados,
        //            productosFiltrados);

        //        TempData["Success"] = "Cita registrada correctamente.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        // ⚠️ Si hay solapamiento u otro error de negocio
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        await CargarListas(vm);
        //        return View(vm);
        //    }
        //}

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

        //nuevo
        // using VenusBeautyStore.PL.Models;  // para el VM del carrito

        // GET /Citas/ReservarServicio?idServicio=...&idCita=...
        [Authorize]
        public async Task<IActionResult> ReservarServicio(int idServicio, int? idCita = null)
        {
            var userId = _userManager.GetUserId(User)!;

            var citaId = await _citaService.ResolverCitaClienteAsync(userId, idCita);
            if (!citaId.HasValue)
            {
                // No hay cita seleccionada o no pertenece al cliente → seleccionar
                return RedirectToAction(nameof(SeleccionarCita), new { idServicio });
            }

            await _citaService.AgregarServicioAsync(citaId.Value, idServicio, userId);
            return RedirectToAction(nameof(MiCarrito), new { id = citaId.Value });
        }

        [Authorize]
        public async Task<IActionResult> ReservarProducto(int idProducto, int cantidad = 1, int? idCita = null)
        {
            var userId = _userManager.GetUserId(User)!;

            var citaId = await _citaService.ResolverCitaClienteAsync(userId, idCita);
            if (!citaId.HasValue)
            {
                return RedirectToAction(nameof(SeleccionarCita), new { idProducto, cantidad });
            }

            await _citaService.AgregarProductoAsync(citaId.Value, idProducto, cantidad, userId);
            return RedirectToAction(nameof(MiCarrito), new { id = citaId.Value });
        }

        // Muestra las citas "abiertas" del cliente para elegir
        //[Authorize(Roles = "Cliente")]
        [Authorize]
        public async Task<IActionResult> SeleccionarCita(int? idServicio = null, int? idProducto = null, int cantidad = 1)
        {
            var userId = _userManager.GetUserId(User)!;
            var citas = await _citaService.ObtenerCitasDelClienteAsync(userId, soloAbiertas: true);

            ViewBag.IdServicio = idServicio;
            ViewBag.IdProducto = idProducto;
            ViewBag.Cantidad = cantidad;

            return View(citas);
        }

        // Carrito: resumen de servicios y productos de la cita
        [Authorize]
        public async Task<IActionResult> MiCarrito(int id)
        {
            var userId = _userManager.GetUserId(User)!;

            var cita = await _citaService.ObtenerCitaAsync(id);
            if (cita is null) return NotFound();

            // Seguridad: Cliente dueño o Estilista asignado o Admin/Recepcionista
            var esDueno = cita.Cliente?.UserId == userId;
            var esEstilista = cita.IdUsuario == userId;
            var esStaff = User.IsInRole("Admin") || User.IsInRole("Recepcionista");

            if (!esDueno && !esEstilista && !esStaff) return Forbid();

            var vm = new CitaResumenViewModel
            {
                IdCita = cita.IdCita,
                FechaHora = cita.FechaHora,
                NombreTrabajador = cita.Trabajador != null ? $"{cita.Trabajador.Nombre} {cita.Trabajador.Apellido}" : "(sin estilista)",
                Total = cita.Total,
                Servicios = cita.DetalleCitas.Select(d => new ItemServicioVM
                {
                    IdServicio = d.IdServicio,
                    Nombre = d.Servicio?.Nombre ?? $"Servicio #{d.IdServicio}",
                    Precio = d.Servicio?.Precio ?? 0m
                }).ToList(),
                Productos = cita.ReservaProductos.Select(r => new ItemProductoVM
                {
                    IdProducto = r.IdProducto,
                    Nombre = r.Producto?.Nombre ?? $"Producto #{r.IdProducto}",
                    Cantidad = r.Cantidad,
                    Subtotal = r.PrecioUnitario * r.Cantidad
                }).ToList()
            };

            return View(vm);
        }

        [Authorize]
        public async Task<IActionResult> MiCarritoActual()
        {
            var userId = _userManager.GetUserId(User)!;

            var abierta = (await _citaService.ObtenerCitasDelClienteAsync(userId, soloAbiertas: true))
                          .FirstOrDefault();
            if (abierta == null)
            {
                TempData["Info"] = "No tienes una cita abierta. Crea una nueva para empezar.";
                return RedirectToAction(nameof(Create));
            }
            return RedirectToAction(nameof(MiCarrito), new { id = abierta.IdCita });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarServicio(int idCita, int idServicio)
        {
            var userId = _userManager.GetUserId(User)!;
            try { await _citaService.QuitarServicioAsync(idCita, idServicio, userId); TempData["Success"] = "Servicio eliminado."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(MiCarrito), new { id = idCita });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int idCita, int idProducto)
        {
            var userId = _userManager.GetUserId(User)!;
            try { await _citaService.QuitarProductoAsync(idCita, idProducto, userId); TempData["Success"] = "Producto eliminado."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(MiCarrito), new { id = idCita });
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCantidadProducto(int idCita, int idProducto, int cantidad)
        {
            var userId = _userManager.GetUserId(User)!;
            try { await _citaService.ActualizarCantidadProductoAsync(idCita, idProducto, cantidad, userId); TempData["Success"] = "Cantidad actualizada."; }
            catch (Exception ex) { TempData["Error"] = ex.Message; }
            return RedirectToAction(nameof(MiCarrito), new { id = idCita });
        }

        // Confirmar cita
        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(int id)
        {
            try
            {
                await _citaService.ActualizarEstadoAsync(id, EstadoCita.Confirmada);
                TempData["Success"] = "Cita confirmada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(MiCarrito), new { id });
            }
        }


    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VenusBeautyStore.PL.Models;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize]
    public class AgendaController : Controller
    {
        private readonly ICitaService _citaService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AgendaController(ICitaService citaService,
                        UserManager<ApplicationUser> userManager)
        {
            _citaService = citaService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();          // buscará Views/Agenda/Index.cshtml
        }

        // GET: /Agenda/Dia
        public async Task<IActionResult> Dia(DateTime? fecha = null, string? estilistaId = null)
        {
            var dia = (fecha ?? DateTime.Today).Date;

            if (User.IsInRole("Estilista") &&
                !User.IsInRole("Admin") &&
                !User.IsInRole("Recepcionista"))
            {
                estilistaId = _userManager.GetUserId(User);
            }

            DateTime desde = dia;
            DateTime hasta = dia.AddDays(1).AddTicks(-1);

            var citas = await _citaService.ObtenerAgendaAsync(desde, hasta, estilistaId);

            var filas = citas.Select(c => new AgendaCitaDto
            {
                Hora = c.FechaHora,
                Cliente = $"{c.Cliente?.Nombre} {c.Cliente?.Apellido1}",
                Estilista = $"{c.Trabajador?.Nombre} {c.Trabajador?.Apellido}",
                Servicios = string.Join(", ", c.DetalleCitas.Select(d => d.Servicio?.Nombre)),
                Productos = string.Join(", ", c.ReservaProductos
                                                .Select(r => $"{r.Producto?.Nombre} ({r.Cantidad})")),
                Estado = c.Estado
            }).OrderBy(f => f.Hora).ToList();

            var vm = new AgendaDiaViewModel
            {
                Fecha = dia,
                Citas = filas
            };

            return View("Dia", vm);   // buscará Views/Agenda/Dia.cshtml
        }
        [Authorize]
        public async Task<IActionResult> Semana(DateTime? inicio = null, string? estilistaId = null)
        {
            var first = (inicio ?? DateTime.Today).Date;
            while (first.DayOfWeek != DayOfWeek.Monday)   // retrocede hasta lunes
                first = first.AddDays(-1);

            // 2) Si es estilista, fuerza su propio Id
            if (User.IsInRole("Estilista") &&
                !User.IsInRole("Admin") &&
                !User.IsInRole("Recepcionista"))
            {
                estilistaId = _userManager.GetUserId(User);
            }

            // 3) Generar los 7 días
            var dias = new List<AgendaDiaViewModel>();

            for (int i = 0; i < 7; i++)
            {
                var dia = first.AddDays(i);
                var desde = dia;
                var hasta = dia.AddDays(1).AddTicks(-1);

                var citas = await _citaService.ObtenerAgendaAsync(desde, hasta, estilistaId);

                var filas = citas.Select(c => new AgendaCitaDto
                {
                    Hora = c.FechaHora,
                    Cliente = $"{c.Cliente?.Nombre} {c.Cliente?.Apellido1}",
                    Estilista = $"{c.Trabajador?.Nombre} {c.Trabajador?.Apellido}",
                    Servicios = string.Join(", ", c.DetalleCitas.Select(d => d.Servicio?.Nombre)),
                    Productos = string.Join(", ", c.ReservaProductos
                                                    .Select(r => $"{r.Producto?.Nombre} ({r.Cantidad})")),
                    Estado = c.Estado
                }).OrderBy(f => f.Hora).ToList();

                dias.Add(new AgendaDiaViewModel
                {
                    Fecha = dia,
                    Citas = filas
                });
            }

            var vm = new AgendaSemanalViewModel
            {
                Inicio = first,
                Dias = dias
            };

            return View("Semana", vm);   // apuntará a Views/Agenda/Semana.cshtml
        }
    }
}

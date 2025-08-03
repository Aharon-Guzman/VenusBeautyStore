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
        // GET: /Agenda/Mes?inicio=2025-08-01
        [Authorize]
        public async Task<IActionResult> Mes(DateTime? inicio = null, string? estilistaId = null)
        {
            // 1️⃣ Primer día del mes
            var mes = inicio?.Date ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // 2️⃣ Si es estilista, usamos su propio ID
            if (User.IsInRole("Estilista") &&
                !User.IsInRole("Admin") &&
                !User.IsInRole("Recepcionista"))
            {
                estilistaId = _userManager.GetUserId(User);
            }

            // 3️⃣ Obtener cantidad de días en el mes
            int diasEnMes = DateTime.DaysInMonth(mes.Year, mes.Month);

            // 4️⃣ Crear lista de días con citas
            var dias = new List<AgendaMesViewModel.DiaCalendarioDto>();
            for (int i = 1; i <= diasEnMes; i++)
            {
                var dia = new DateTime(mes.Year, mes.Month, i);
                var desde = dia;
                var hasta = dia.AddDays(1).AddTicks(-1);

                var citas = await _citaService.ObtenerAgendaAsync(desde, hasta, estilistaId);

                var filas = citas.Select(c => new AgendaCitaDto
                {
                    Hora = c.FechaHora,
                    Cliente = $"{c.Cliente?.Nombre} {c.Cliente?.Apellido1}",
                    Estilista = $"{c.Trabajador?.Nombre} {c.Trabajador?.Apellido}",
                    Servicios = string.Join(", ", c.DetalleCitas.Select(d => d.Servicio?.Nombre)),
                    Productos = string.Join(", ", c.ReservaProductos.Select(r => $"{r.Producto?.Nombre} ({r.Cantidad})")),
                    Estado = c.Estado
                }).OrderBy(f => f.Hora).ToList();

                dias.Add(new AgendaMesViewModel.DiaCalendarioDto
                {
                    Fecha = dia,
                    Citas = filas
                });
            }

            // 5️⃣ Armar cuadrícula de 6 semanas x 7 días
            var semanas = new List<List<AgendaMesViewModel.DiaCalendarioDto>>();
            var semanaActual = new List<AgendaMesViewModel.DiaCalendarioDto>();

            // Calcular en qué día de la semana inicia (lunes=1, domingo=7)
            int inicioSemana = mes.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)mes.DayOfWeek;


            // Agregar celdas vacías antes del primer día
            for (int i = 1; i < inicioSemana; i++)
            {
                semanaActual.Add(new AgendaMesViewModel.DiaCalendarioDto());
            }

            // Llenar días reales
            foreach (var d in dias)
            {
                semanaActual.Add(d);
                if (semanaActual.Count == 7)
                {
                    semanas.Add(semanaActual);
                    semanaActual = new List<AgendaMesViewModel.DiaCalendarioDto>();
                }
            }

            // Completar última semana con celdas vacías si es necesario
            if (semanaActual.Count > 0)
            {
                while (semanaActual.Count < 7)
                    semanaActual.Add(new AgendaMesViewModel.DiaCalendarioDto());

                semanas.Add(semanaActual);
            }

            var vm = new AgendaMesViewModel
            {
                Mes = mes,
                Semanas = semanas
            };

            return View("Mes", vm);
        }

    }
}

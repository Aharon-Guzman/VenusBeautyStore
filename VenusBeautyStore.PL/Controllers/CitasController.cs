using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.BLL.Services;  // ICitaService
using System.Threading.Tasks;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        // GET: /Citas
        public async Task<IActionResult> Index()
        {
            var citas = await _citaService.ObtenerAgendaAsync(
                DateTime.Today.AddDays(-30),  // rango amplio por ahora
                DateTime.Today.AddDays(30));
            return View(citas);
        }
    }
}

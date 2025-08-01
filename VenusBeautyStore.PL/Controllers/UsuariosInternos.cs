using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class UsuariosInternos : Controller
    {
        private readonly ITrabajadorService _trabajadorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosInternos(ITrabajadorService trabajadorService, UserManager<ApplicationUser> userManager)
        {
            _trabajadorService = trabajadorService;
            _userManager = userManager;
        }

        // ✅ Listar todos los trabajadores
        public async Task<IActionResult> Index()
        {
            var trabajadores = await _trabajadorService.ObtenerTrabajadoresAsync();
            return View(trabajadores);
        }

        // ✅ Vista para crear nuevo usuario interno
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.RolesDisponibles = ObtenerRolesDisponibles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trabajador trabajador, string Password, string Email)
        {
            ViewBag.RolesDisponibles = ObtenerRolesDisponibles();

            if (!ModelState.IsValid)
                return View(trabajador);

            var (creado, errores) = await _trabajadorService.CrearTrabajadorAsync(
                trabajador,
                Password,
                trabajador.Rol,
                Email
            );

            if (!creado)
            {
                foreach (var error in errores)
                    ModelState.AddModelError("", error);

                return View(trabajador);
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ Vista para editar trabajador
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var trabajador = await _trabajadorService.ObtenerPorIdAsync(id);
            if (trabajador == null)
                return NotFound();

            return View(trabajador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Trabajador trabajador, string? NewPassword)
        {
            if (!ModelState.IsValid)
                return View(trabajador);

            var trabajadorDb = await _trabajadorService.ObtenerPorIdAsync(id);
            if (trabajadorDb == null)
                return NotFound();

            if (User.IsInRole("Recepcionista"))
                trabajador.Rol = trabajadorDb.Rol;

            trabajadorDb.Nombre = trabajador.Nombre;
            trabajadorDb.Apellido = trabajador.Apellido;
            trabajadorDb.Telefono = trabajador.Telefono;

            if (User.IsInRole("Admin"))
                trabajadorDb.Rol = trabajador.Rol;

            var actualizado = await _trabajadorService.EditarTrabajadorAsync(id, trabajadorDb);

            if (actualizado && User.IsInRole("Admin") && !string.IsNullOrWhiteSpace(NewPassword))
                await _trabajadorService.CambiarPasswordAsync(trabajadorDb.UserId, NewPassword);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var trabajador = await _trabajadorService.ObtenerPorIdAsync(id);
            if (trabajador == null)
                return NotFound();

            return View(trabajador);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _trabajadorService.EliminarTrabajadorAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Método privado para no repetir código
        private List<string> ObtenerRolesDisponibles()
        {
            if (User.IsInRole("Admin"))
                return new List<string> { "Admin", "Recepcionista", "Estilista" };

            if (User.IsInRole("Recepcionista"))
                return new List<string> { "Estilista" };

            return new List<string>();
        }
    }
}

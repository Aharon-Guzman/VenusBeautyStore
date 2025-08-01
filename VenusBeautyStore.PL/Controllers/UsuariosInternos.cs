using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VenusBeauty.BLL.Services;
using VenusBeauty.DAL.Entities;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class UsuariosInternos : Controller
    {
        private readonly ITrabajadorService _trabajadorService;

        public UsuariosInternos(ITrabajadorService trabajadorService)
        {
            _trabajadorService = trabajadorService;
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
            List<string> rolesDisponibles = new();

            if (User.IsInRole("Admin"))
            {
                rolesDisponibles.AddRange(new[] { "Admin", "Recepcionista", "Estilista" });
            }
            else if (User.IsInRole("Recepcionista"))
            {
                rolesDisponibles.Add("Estilista");
            }

            ViewBag.RolesDisponibles = rolesDisponibles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Trabajador trabajador, string Password)
        {
            // 🔹 Volvemos a cargar los roles disponibles SIEMPRE
            List<string> rolesDisponibles = new();
            if (User.IsInRole("Admin"))
                rolesDisponibles.AddRange(new[] { "Admin", "Recepcionista", "Estilista" });
            else if (User.IsInRole("Recepcionista"))
                rolesDisponibles.Add("Estilista");

            ViewBag.RolesDisponibles = rolesDisponibles;

            if (!ModelState.IsValid)
                return View(trabajador);

            var creado = await _trabajadorService.CrearTrabajadorAsync(trabajador, Password, trabajador.Rol);
            if (!creado)
            {
                ModelState.AddModelError("", "❌ Error al crear el usuario interno.");
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

            // 🔹 Verificamos si el trabajador existe
            var trabajadorDb = await _trabajadorService.ObtenerPorIdAsync(id);
            if (trabajadorDb == null)
                return NotFound();

            // 🔹 Si el usuario logueado es Recepcionista → bloquear cambio de Rol
            if (User.IsInRole("Recepcionista"))
            {
                trabajador.Rol = trabajadorDb.Rol; // Mantener el rol original
            }

            // 🔹 Actualizamos datos básicos
            trabajadorDb.Nombre = trabajador.Nombre;
            trabajadorDb.Apellido = trabajador.Apellido;
            trabajadorDb.Telefono = trabajador.Telefono;

            // 🔹 Si el usuario actual es Admin, puede cambiar el Rol
            if (User.IsInRole("Admin"))
            {
                trabajadorDb.Rol = trabajador.Rol;
            }

            // 🔹 Guardamos cambios del trabajador
            var actualizado = await _trabajadorService.EditarTrabajadorAsync(id, trabajadorDb);

            // 🔹 Si es Admin y envió una nueva contraseña
            if (actualizado && User.IsInRole("Admin") && !string.IsNullOrWhiteSpace(NewPassword))
            {
                await _trabajadorService.CambiarPasswordAsync(trabajadorDb.UserId, NewPassword);
            }

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

    }
}

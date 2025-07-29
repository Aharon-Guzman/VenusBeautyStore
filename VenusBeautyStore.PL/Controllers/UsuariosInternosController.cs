using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VenusBeauty.BLL.Services;
using VenusBeautyStore.PL.Models.ViewModels;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista")]
    public class UsuariosInternosController : Controller
    {
        private readonly IUsuarioInternoService _usuarioInternoService;

        public UsuariosInternosController(IUsuarioInternoService usuarioInternoService)
        {
            _usuarioInternoService = usuarioInternoService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioInternoService.ObtenerUsuariosInternosAsync();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var roles = new List<string>();

            if (User.IsInRole("Admin"))
            {
                roles.AddRange(new[] { "Admin", "Recepcionista", "Estilista" });
            }
            else if (User.IsInRole("Recepcionista"))
            {
                roles.AddRange(new[] { "Recepcionista", "Estilista" });
            }

            ViewBag.Roles = roles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioInternoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _usuarioInternoService.CrearUsuarioInternoAsync(
                model.Email,
                model.Password,
                model.Rol,
                model.Nombre,
                model.Apellido,
                model.Telefono
            );

            if (!resultado)
            {
                ModelState.AddModelError("", "Error al crear el usuario");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

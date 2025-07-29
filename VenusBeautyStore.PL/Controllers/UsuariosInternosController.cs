using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VenusBeauty.BLL.Services;
using VenusBeautyStore.PL.Models;
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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
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
                model.Rol
            );

            if (!resultado)
            {
                ModelState.AddModelError("", "Error al crear el usuario");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

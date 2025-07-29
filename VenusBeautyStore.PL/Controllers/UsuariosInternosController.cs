using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace VenusBeautyStore.PL.Controllers
{
    
    [Authorize(Roles = "Admin,Recepcionista")]
    public class UsuariosInternosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosInternosController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Acción GET para mostrar el formulario
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Acción POST para recibir datos del formulario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string dummy)
        {
            // Aquí más adelante vamos a recibir los datos del formulario
            // y crear el usuario con Identity
            return View();
        }

    }
}

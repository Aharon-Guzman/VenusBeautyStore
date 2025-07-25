using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;
using VenusBeauty.DAL.Entities;


namespace VenusBeautyStore.PL.Controllers
{
    [Authorize (Roles ="Admin,Recepcionista")]
    public class ClientesController : Controller
    {
        private readonly VenusBeautyContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientesController(VenusBeautyContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente, string Password)
        {
            if (!ModelState.IsValid)
                return View(cliente);

            // ✅ 1. Crear el usuario en Identity
            var user = new IdentityUser
            {
                UserName = cliente.Correo,
                Email = cliente.Correo,
                EmailConfirmed = true // El admin/recepcionista lo confirma automáticamente
            };

            var result = await _userManager.CreateAsync(user, Password);


            if (result.Succeeded)
            {
                // ✅ 2. Crear entrada en tabla Clientes
                cliente.UserId = user.Id;
                cliente.Activo = true;

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // ❌ Si hubo errores, mostrarlos
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(cliente);

        }

    }
}

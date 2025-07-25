using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;


namespace VenusBeautyStore.PL.Controllers
{
    [Authorize (Roles ="Admin,Recepcionista")]
    public class ClientesController : Controller
    {
        private readonly VenusBeautyContext _context;

        public ClientesController(VenusBeautyContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
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
    }
}

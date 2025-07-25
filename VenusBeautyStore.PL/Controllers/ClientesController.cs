using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VenusBeauty.DAL.Context;


namespace VenusBeautyStore.PL.Controllers
{
    [Authorize (Roles ="Admin,Recepcionista")]
    public class ClientesController : Controller
    {
        private readonly VenusBeautyContext _context;

        public ClientesController(VenusBeautyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return View(clientes);
        }
    }
}

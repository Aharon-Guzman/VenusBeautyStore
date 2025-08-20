using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VenusBeautyStore.PL.Controllers
{
    [Authorize(Roles = "Admin,Recepcionista,Estilista")]
    public class PortalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

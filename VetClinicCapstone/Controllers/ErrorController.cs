using Microsoft.AspNetCore.Mvc;

namespace VetClinicCapstone.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

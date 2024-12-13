using Microsoft.AspNetCore.Mvc;

namespace VetClinicCapstone.Controllers.Security
{
    public class RegisterAccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

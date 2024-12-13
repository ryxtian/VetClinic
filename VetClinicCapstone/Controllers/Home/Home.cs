using Microsoft.AspNetCore.Mvc;

namespace VetClinicCapstone.Controllers.Home
{
	public class Home : Controller
	{
		public IActionResult Index()
		{
			return View("../HomePage/Home");
		}
	}
}

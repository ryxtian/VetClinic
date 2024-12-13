using Microsoft.AspNetCore.Mvc;

namespace VetClinicCapstone.Controllers.Doctor
{
	public class InventoryController : Controller
	{
		public IActionResult List()
		{
			return View("../Doctor/Inventory/List");
		}
	}
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;
using VetClinicCapstone.Models;
using VetClinicCapstone.Repository;


namespace VetClinicCapstone.Controllers.Login
{
	public class AdminLoginController : Controller
    {
		private readonly IRepository<AdminTbl> _adminRepo;
		private readonly ApplicationDbContext _context;

		public AdminLoginController(IRepository<AdminTbl> adminRepo, ApplicationDbContext context)
		{
			_adminRepo = adminRepo;
			_context = context;
		}
		
		public async Task<IActionResult> Index()
		{
			return View();
        }


		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{

			var user = await _adminRepo.GetFirstOrDefaultAsync(u => u.Username == username && u.Password == password);
			if (user != null)
			{
				var adminDetail = await _context.AdminInfoTbls
					.Where(od => od.AdminInfo_ID == user.AdminInfo_ID)
					.FirstOrDefaultAsync();

				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, adminDetail.AdminInfo_ID.ToString()),
			new Claim(ClaimTypes.Name, adminDetail.Firstname),
			new Claim(ClaimTypes.Role, adminDetail.Role),
				};

				var claimsIdentity = new ClaimsIdentity(claims, "AdminScheme");
				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

				await HttpContext.SignInAsync("AdminScheme", claimsPrincipal);

				TempData["SuccessMessage"] = "Welcome, successfully logged in!";

				return RedirectToAction("Index", "AdminDashboard");
			}

			ViewBag.Error = "Invalid username or password";
			return View("../AdminLogin/Index");
		}


		//[HttpPost]
		//public async Task<IActionResult> Login(string username, string password)
		//{
		//	var user = await _adminRepo.GetFirstOrDefaultAsync(u => u.Username == username && u.Password == password);
		//	if (user != null)
		//	{
		//		var AdminDetaile = await _context.AdminInfoTbls.Where(od => od.AdminInfo_ID == user.AdminInfo_ID).FirstOrDefaultAsync();
		//		var claims = new List<Claim>
		//{
		//	new Claim(ClaimTypes.NameIdentifier, AdminDetaile.AdminInfo_ID.ToString()),
		//	new Claim(ClaimTypes.Name, AdminDetaile.Firstname),
		//	new Claim(ClaimTypes.Role, AdminDetaile.Role)
		//	  };

		//		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		//		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

		//		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
		//		TempData["SuccessMessage"] = "Welcome successfully login!";

		//		return RedirectToAction("Index", "AdminDashboard");
		//	}

		//	ViewBag.Error = "Invalid username or password";

		//	return View("../AdminLogin/Index");
		//}

		public async Task<IActionResult> Logout()
        {
			await HttpContext.SignOutAsync("AdminScheme");
			return RedirectToAction("Index", "AdminLogin");	
        }
    }
}

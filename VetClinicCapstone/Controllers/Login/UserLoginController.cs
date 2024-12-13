using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VetClinicCapstone.Models;
using VetClinicCapstone.Repository;
using VetClinicCapstone.Models.ViewModel;


namespace VetClinicCapstone.Controllers.Login
{

	public class UserLoginController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IRepository<UserTbl> _userRepo;
        private readonly IRepository<OwnerTbl> _ownerRepo;


        public UserLoginController(IRepository<UserTbl> userRepo, ApplicationDbContext context, IRepository<OwnerTbl> ownerRepo)
		{
			_userRepo = userRepo;
			_context = context;
            _ownerRepo = ownerRepo;
        }

		public IActionResult Index()
		{
            //// Log the authentication state
            //if (User.Identity.IsAuthenticated)
            //{

            //    return RedirectToAction("Index","UserDashboard");
            //}

            return View("../UserLogin/Index");
		}

		public static class PasswordHasher
		{
			public static string HashPassword(string password)
			{
				using (var sha256 = SHA256.Create())
				{
					var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
					return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
				}
			}
		}

		[HttpPost]
		public async Task<IActionResult> Register(UserAndOwnerViewModel model)
		{

			var owner = new OwnerTbl
			{
				Firstname = model.OwnerDetail.Firstname,
				Midname = model.OwnerDetail.Midname,
				Lastname = model.OwnerDetail.Lastname,
				SuffixName = model.OwnerDetail.SuffixName,
				Email = model.OwnerDetail.Email,
				Phone = model.OwnerDetail.Phone,
				Province = model.OwnerDetail.Province,
				City = model.OwnerDetail.City,
				Baranggay = model.OwnerDetail.Baranggay,
				Street = model.OwnerDetail.Street,
				Sex = model.OwnerDetail.Sex,
				//CreatedAt = DateTime.Now,
			};
			await _userRepo.AddOwnerAsync(owner);
			await _userRepo.SaveChangesAsync();

			var user = new UserTbl
			{
				Password = PasswordHasher.HashPassword(model.UserDetail.Password),
				ConfirmPassword = PasswordHasher.HashPassword(model.UserDetail.ConfirmPassword),
				DateRegistered = DateTime.Now,
				Roles = "User",
				Owner_ID = owner.Owner_ID
			};



			await _userRepo.AddUserAsync(user);
			await _userRepo.SaveChangesAsync();

			return RedirectToAction("Index");
		}

        [HttpPost]
        public async Task<IActionResult> Login(UserAndOwnerViewModel user)
        {
            // Find the owner by email
            var owner = await _context.OwnerTbls
                .Where(o => o.Email == user.OwnerDetail.Email)
                .Select(o => new
                {
                    o.Owner_ID,
                    o.Firstname,
                    o.Lastname,
                    o.Email
                })
                .FirstOrDefaultAsync();

            if (owner != null)
            {
                // Retrieve the user details by Owner_ID
                var userDetails = await _context.UserTbls
                    .Where(u => u.Owner_ID == owner.Owner_ID)
                    .Select(u => new
                    {
                        u.Password,
                        u.IsActivated
                    })
                    .FirstOrDefaultAsync();

                // Ensure that the user details and the password are valid
                if (userDetails != null && !string.IsNullOrEmpty(user.UserDetail.Password) && userDetails.Password == PasswordHasher.HashPassword(user.UserDetail.Password) && userDetails.IsActivated == 1)
                {
                    // Create claims for the user
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, owner.Owner_ID.ToString()),
                new Claim(ClaimTypes.Name, owner.Firstname),
                new Claim(ClaimTypes.Surname, owner.Lastname),
                new Claim(ClaimTypes.Email, owner.Email),
                new Claim(ClaimTypes.Role, "User")
            };

                    // Create the ClaimsIdentity and ClaimsPrincipal
                    var claimsIdentity = new ClaimsIdentity(claims, "UserScheme");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Sign in the user
                    await HttpContext.SignInAsync("UserScheme", claimsPrincipal);
                    return RedirectToAction("Index", "UserDashboard");
                }
            }

            // Set an error message and return the login view
            TempData["InvalidLogin"] = "Invalid E-mail or password. Please try again.";
            return View("Index");
        }


        //[HttpPost]
        //public async Task<IActionResult> Login(UserAndOwnerViewModel user)
        //{
        //    var owner = await _context.OwnerTbls
        //        .Where(o => o.Email == user.OwnerDetail.Email)
        //        .Select(o => new
        //        {
        //            o.Owner_ID,
        //            o.Firstname,
        //            o.Lastname,
        //            o.Email
        //        })
        //        .FirstOrDefaultAsync();

        //    if (owner != null)
        //    {
        //        // Retrieve the user details by Owner_ID
        //        var userDetails = await _context.UserTbls
        //            .Where(u => u.Owner_ID == owner.Owner_ID)
        //            .Select(u => new
        //            {
        //                u.Password,
        //                u.IsActivated
        //            })
        //            .FirstOrDefaultAsync();

        //        if (userDetails != null && userDetails.Password == PasswordHasher.HashPassword(user.UserDetail.Password) && userDetails.IsActivated == 1)
        //        {
        //            // Create claims for the user
        //            var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, owner.Owner_ID.ToString()),
        //        new Claim(ClaimTypes.Name, owner.Firstname),
        //        new Claim(ClaimTypes.Surname, owner.Lastname),
        //        new Claim(ClaimTypes.Email, owner.Email),
        //        new Claim(ClaimTypes.Role, "User")
        //    };

        //            // Create the ClaimsIdentity and ClaimsPrincipal
        //            var claimsIdentity = new ClaimsIdentity(claims, "UserScheme");
        //            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //            // Sign in the user
        //            await HttpContext.SignInAsync("UserScheme", claimsPrincipal);
        //            return RedirectToAction("Index", "UserDashboard");
        //        }
        //    }

        //    // Set an error message and return the login view
        //    TempData["InvalidLogin"] = "Invalid E-mail or password. Please try again.";
        //    return View("Index");
        //}
        public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("UserScheme");
			return RedirectToAction("Index", "Userlogin");
		}
	}
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;

namespace VetClinicCapstone.Controllers.UserProfile
{
    [Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
    public class UserProfile : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserProfile(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // Get the current user ID from claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Convert userId to integer (assuming userId is numeric)
            if (!int.TryParse(userId, out int userIdInt))
            {
                return BadRequest("Invalid user ID.");
            }

            // Retrieve the user details and related owner information
            var userProfile = await (from user in _context.UserTbls
                                     join owner in _context.OwnerTbls
                                     on user.Owner_ID equals owner.Owner_ID
                                     where user.Owner_ID == userIdInt
                                     select new UserAndOwnerViewModel
                                     {
                                         UserDetail = user,
                                         OwnerDetail = owner
                                     })
                                    .FirstOrDefaultAsync();

            if (userProfile == null)
            {
                return NotFound();
            }

            return View("../User/UserProfile/index",userProfile);  // Pass the ViewModel to the view
        }
    }
}
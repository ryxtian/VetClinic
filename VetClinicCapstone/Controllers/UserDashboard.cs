using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;

namespace VetClinicCapstone.Controllers
{
	[Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
	public class UserDashboard : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserDashboard(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userIDString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIDString, out var userID))
            {
                // Handle invalid userID scenario
                return RedirectToAction("Error", "Home");
            }

            var today = DateTime.Today;
            var threeDaysFromNow = today.AddDays(5);

            var appointments = await (from a in _context.AppointmentTbls
                                      join p in _context.PatientTbls
                                      on a.Patient_ID equals p.Patient_ID
                                      where a.Owner_ID == userID &&
                                            a.Date >= today &&
                                            a.Date <= threeDaysFromNow &&
                                            a.Status == "Approved"
                                      select new
                                      {
                                          a,
                                          p
                                      }).ToListAsync();

            ViewBag.UpcomingAppointments = appointments;

            return View("../Dashboard/UserDashboard");
        }



        public IActionResult CreateAppoint()
        {
            return View("../User/Appointment/CreateAppoint");
        }
    }
}

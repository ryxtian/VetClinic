using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Models;
using VetClinicCapstone.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Linq;
using VetClinicCapstone.EmailNotication;
using Microsoft.AspNetCore.Authorization;

namespace VetClinicCapstone.Controllers.Appointment
{

    public class AppointSchedule : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IRepository<AppointmentTbl> _appointRepo;
		private readonly IRepository<PatientTbl> _patientRepo;
		private readonly IRepository<OwnerTbl> _ownerRepo;
		private readonly IRepository<ConsultationTbl> _consultRepo;
        private readonly IEmailService _emailService;
        public AppointSchedule(IRepository<AppointmentTbl> appointRepo,IRepository<ConsultationTbl> consultRepo, ApplicationDbContext context, IRepository<PatientTbl> patientRepo, IRepository<OwnerTbl> ownerRepo, IEmailService emailService)
		{
			_context = context;
			_appointRepo = appointRepo;
			_patientRepo = patientRepo;
			_consultRepo = consultRepo;
			_ownerRepo = ownerRepo;
            _emailService = emailService;
        }
		[Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
		public IActionResult List()
		{
			var UserID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var appointlist = from appoint in _appointRepo.GetAll()
							  join patient in _patientRepo.GetAll() on appoint.Patient_ID equals patient.Patient_ID
							  join owner in _ownerRepo.GetAll() on patient.Owner_ID equals owner.Owner_ID
								  where patient.Owner_ID == Convert.ToInt32(UserID)

								  select new JoinAppointPatientOwner
							  {
								  AppointmentTbls = appoint,
								  PatientTbls = patient,
								  OwnerTbls = owner

							  };

			return View("../User/Appointment/List", appointlist.ToList());
		}

		[HttpGet]
		[Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
		public IActionResult CreateAppoint()
		{
			var ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int PetCount = _context.AppointmentTbls.Where(s => s.Status == "Completed" && s.Owner_ID == Convert.ToInt32(ID)).Count();

            //		int PetCount = _context.PatientTbls
            //.Where(p => p.Owner_ID == Convert.ToInt32(ID))
            //.SelectMany(p => p.AppointmentTbls)

            //.Select(a => a.Patient_ID)
            //.Distinct()
            //.Count();
            int ApproveCount = _context.AppointmentTbls.Where(s=>s.Status=="Approved" && s.Owner_ID == Convert.ToInt32(ID)).Count();
			int ForApproveCount = _context.AppointmentTbls.Where(s => s.Status == "For Approval" && s.Owner_ID == Convert.ToInt32(ID)).Count();
			int CancelledCount = _context.AppointmentTbls.Where(s => s.Status == "Cancelled" && s.Owner_ID == Convert.ToInt32(ID)).Count();

			ViewBag.PetCount = PetCount;
			ViewBag.ApproveCount = ApproveCount;
			ViewBag.ForApproveCount = ForApproveCount;
			ViewBag.CancelledCount = CancelledCount;



			var getDoctorSched = _context.AppointmentScheduleTbls.ToList();
			var getJoin = (from pet in _context.PatientTbls

							   join appoint in _context.AppointmentTbls
							   on pet.Patient_ID equals appoint.Patient_ID
								join owner in _context.OwnerTbls
								on appoint.Owner_ID equals owner.Owner_ID
						   where pet.Owner_ID == Convert.ToInt32(ID)&&appoint.Status !="Cancelled"
						   select new AppointmentPatientViewModel
							   {
								   Patient = pet,
								   Appointment = appoint,

							   }).ToList();
			var service = _context.ServiceTbls.Where(i=>i.IsDeleted == 0).ToList();
			var pets = _context.PatientTbls.Where(i => i.Owner_ID == Convert.ToInt32(ID)&&i.IsDeleted==0).ToList();///xxxxxxxxxxxxxxxxxxxxxxxx

			var result = new AppointmentPatientViewModel()
			{
				AppointmentPatientViewModelList = getJoin,
				AppointmentScheduleTbls = getDoctorSched,
				ServiceList = service,
				PatientList = pets
			};
			return View("../User/Appointment/CreateAppoint", result);
		}
		[HttpGet]
		public IActionResult GetTakenTimes(DateTime date)
		{
			
			var takenTimes = _context.AppointmentTbls
				.Where(a => a.Date.Date == date.Date && a.Status !="Cancelled")//xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
				.Select(a => a.Time)
				.ToList();

			return Ok(takenTimes);
		}


		public class AppointmentMultiplePetViewModel
		{
			public DateTime Date { get; set; } // Single date for the appointment

			public List<PetAppointmentViewModel> PetAppointments { get; set; } // List of pets with appointment times
		}

		public class PetAppointmentViewModel
		{
			public string? PetName { get; set; }
			public string? Species { get; set; }
			public string? Breed { get; set; }
			public DateTime? Birthday { get; set; }//x
			public string? Sex { get; set; }//x
			public string? ColorMarking { get; set; }
			public string? Description { get; set; }
			public string? AppointmentTime { get; set; } // Time specific to each pet
			public long? Service_ID { get; set; }
			public long? Patient_ID { get; set; }
		}
		[HttpPost]
		public async Task<IActionResult> CreateAppoint([FromBody] AppointmentMultiplePetViewModel model)
		{
			var message = "";
			var isSuccess = false;
			var title = "";

			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (userIdClaim == null)
				{
					return Json(new { success = false, message = "User is not authenticated.", title = "Authentication failed!" });
				}

				foreach (var petAppointment in model.PetAppointments)
				{
					// Check if an appointment already exists for the given date and time
					//var existingAppointment = await _context.AppointmentTbls
					//	.FirstOrDefaultAsync(a => a.Date == model.Date && a.Time == petAppointment.AppointmentTime);

					//if (existingAppointment != null)
					//{
					//	return Json(new { success = false, message = "Appointment time is already taken.", title = "Appointment failed!" });
					//}

					// Save patient information
					//var patientEntity = new PatientTbl
					//{
					//	PatientName = petAppointment.PetName,
					//	Species = petAppointment.Species,
					//	Breed = petAppointment.Breed,
					//	ColorMarking = petAppointment.ColorMarking,
					//	Birthday = petAppointment.Birthday,//x
					//	Sex = petAppointment.Sex,//x
					//	Description = petAppointment.Description,
					//	Owner_ID = Convert.ToInt32(userIdClaim)
					//};

					//await _patientRepo.AddPatientAsync(patientEntity);
					//await _patientRepo.SaveChangesAsync();



					var appointment = new AppointmentTbl
					{
						Service_ID = petAppointment.Service_ID,
						Date = model.Date,
						Time = petAppointment.AppointmentTime,
						Status = "For Approval",
						Patient_ID = petAppointment.Patient_ID,
						Owner_ID = Convert.ToInt32(userIdClaim)
					};

					await _appointRepo.AddAppointAsync(appointment);
					await _appointRepo.SaveChangesAsync();
				}

				isSuccess = true;
				message = "Successfully saved!";
				title = "Appointments Added!";

				TempData["ToastSuccessMsg"] = "Appointments successfully added.";

				var emailClaim = User.FindFirst(ClaimTypes.Email);
				var nameClaim = User.FindFirst(ClaimTypes.Name);

				string email = emailClaim?.Value;
				string firstName = nameClaim?.Value;

				string subject = "Appointment Request Pending Approval";
				string emailMessage = $@"
Dear {firstName},

Thank you for choosing Urban Vet Animal Clinic for your pet's care. We have received your appointment requests for the following pets on {model.Date:MMMM dd, yyyy} at the following times:

{string.Join("\n", model.PetAppointments.Select(p => $"- Pet's Name: {p.PetName}\n  Species: {p.Species}\n  Breed: {p.Breed}\n  Color Marking: {p.ColorMarking}\n  Description: {p.Description}\n  Appointment Time: {p.AppointmentTime}"))}

Your appointments are currently pending approval. We will review your requests and notify you shortly regarding their status.

If you have any questions or need to modify your requests, please do not hesitate to contact us at 0915 057 8906 or reply to this email.

Thank you for your patience!

Best regards,
Urban Vet Animal Clinic
Dio Commercial Complex
Mabayabas Taysan Batangas
Email: urvanvetanimalclinic@gmail.com
Phone: 0915 057 8906";

				await _emailService.SendEmailAsync(email, subject, emailMessage);
			}
			catch (Exception ex)
			{
				message = "Please ensure the required fields are filled correctly.";
				title = "Error occurred!";
				// Optionally log the exception details
				// _logger.LogError(ex, "An error occurred while saving the appointment.");
			}

			return Json(new { success = isSuccess, message = message, title = title });
		}
		[HttpGet]
		[Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
		public async Task<IActionResult>Cancelled(long ID)
		{

			var del = await _appointRepo.GetByIDAsync(ID);
			if (del != null)
			{
				del.Status = "Cancelled";
				await _appointRepo.SaveChangesAsync();
			}

			return RedirectToAction("CreateAppoint");
		}
		[HttpGet]
		public async Task<IActionResult> GetClinicDays()
		{
			var clinicDays = await _context.AppointmentScheduleTbls.ToListAsync();
			return Json(clinicDays.Select(d => d.Date));
		}

		[HttpGet]
		public JsonResult GetEvents()
		{
			var events = _context.EventsTbls.Select(e => new
			{
				id = e.Event_ID,
				title = e.TItle,
				start = e.Dates,
				allDay = true
			}).ToList();

			return new JsonResult(events);
		}

		[HttpPost]
		public async Task<IActionResult> SaveClinicDays([FromBody] List<string> selectedDays)
		{
			try
			{
				// Update the clinic schedule with selected days
				var allDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

				foreach (var day in allDays)
				{
					var existingSchedule = await _context.AppointmentScheduleTbls.FirstOrDefaultAsync(d => d.Date == day);

					if (selectedDays.Contains(day))
					{
						// Add or update the schedule for selected days
						if (existingSchedule == null)
						{
							_context.AppointmentScheduleTbls.Add(new AppointmentScheduleTbl { Date = day });
						}
					}
					else
					{
						// Remove the schedule for unselected days
						if (existingSchedule != null)
						{
							_context.AppointmentScheduleTbls.Remove(existingSchedule);
						}
					}
				}

				await _context.SaveChangesAsync();

				return Ok(new { success = true, message = "Clinic days updated successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = "Failed to update clinic days.", error = ex.Message });
			}
		}
		public class AppointmentCount
		{
			public string Date { get; set; }
			public int Count { get; set; }
		}

		[HttpGet]
		public async Task<IActionResult> GetAppointmentCounts()
		{
			// Assuming you have an Appointment entity with a Date property
			var appointmentCounts = await _context.AppointmentTbls
				.GroupBy(a => a.Date)
				.Select(g => new AppointmentCount
				{
					Date = g.Key.ToString("yyyy-MM-dd"),
					Count = g.Count()
				})
				.ToListAsync();

			return Json(appointmentCounts);
		}
		[HttpGet]
		public IActionResult CheckAvailability(DateTime date, string time)
		{
			var isAvailable = !_context.AppointmentTbls.Where(s => s.Status != "Cancelled").Any(a => a.Date == date && a.Time == time);//xxxxxxxxxxxxxxxxxxxxx
			return Json(isAvailable);
		}

		[HttpGet]
		public IActionResult GetPetDetails(long patientId)
		{
			if (patientId <= 0)
			{
				return BadRequest("Invalid patient ID.");
			}

			var petDetails = _context.PatientTbls
				.Where(p => p.Patient_ID == patientId)
				.Select(p => new
				{
					PatientName = p.PatientName,
					Species = p.Species,
					Breed = p.Breed,
					Birthday = p.Birthday,
					Sex = p.Sex,
					ColorMarking = p.ColorMarking,
					Description = p.Description
				})
				.ToList();

			if (!petDetails.Any())
			{
				return NotFound();
			}

			return Json(petDetails);
		}
	}
}

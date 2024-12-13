using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Repository;
using static VetClinicCapstone.Controllers.Appointment.AppointSchedule;

namespace VetClinicCapstone.Controllers.MakeAnAppointment
{
	[Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
	public class MakeAnAppointment : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IRepository<AppointmentTbl> _appointRepo;
		private readonly IRepository<OwnerTbl> _ownerRepo;
		private readonly IRepository<PatientTbl> _patientRepo;
		private readonly IRepository<ServiceTbl> _serviceRepo;
		public MakeAnAppointment(ApplicationDbContext context, IRepository<AppointmentTbl> appointRepo, IRepository<OwnerTbl> ownerRepo, IRepository<ServiceTbl> serviceRepo, IRepository<PatientTbl> patientRepo)
		{
			_context = context;
			_appointRepo = appointRepo;
			_ownerRepo = ownerRepo;
			_serviceRepo = serviceRepo;
			_patientRepo = patientRepo;
		}
		[HttpGet]
        public IActionResult Index()
		{
			var getDoctorSched = _context.AppointmentScheduleTbls.ToList();
			var owner = _context.OwnerTbls.ToList();
			var service = _context.ServiceTbls.ToList();
			var pet = _context.PatientTbls.Where(i => i.IsDeleted == 0).ToList();//xxxxxxxxxxxxxxxxxx
			var result = new AppointmentPatientViewModel()
			{
				AppointmentScheduleTbls = getDoctorSched,
				ServiceList = service,
				OwnerList = owner,
				PatientList = pet,
			};
			return View("../MakeAnAppointment/Index", result);
		}
		[HttpGet]
		public IActionResult GetTakenTimes(DateTime date)
		{
			var takenTimes = _context.AppointmentTbls
				.Where(a => a.Date.Date == date.Date && a.Status !="Cancelled")
				.Select(a => a.Time) 
				.ToList();

			return Ok(takenTimes);
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


		public class AppointmentMultiplePetViewModel
		{
			public DateTime Date { get; set; } // Single date for the appointment

			public List<PetAppointmentViewModel> PetAppointments { get; set; } // List of pets with appointment times
		}

		public class PetAppointmentViewModel
		{
			public string PetName { get; set; }
			public string Species { get; set; }
			public string? Breed { get; set; }
			public DateTime? Birthday { get; set; }//x
			public string Sex { get; set; }//x
			public string ColorMarking { get; set; }
			public string? Description { get; set; }
			public string AppointmentTime { get; set; } // Time specific to each pet
			public long Service_ID { get; set; }
			public long Patient_ID { get; set; }
			public long Owner_ID { get; set; }
		}
		[HttpPost]
		public async Task<IActionResult> MakeAppointmentOldClient([FromBody] AppointmentMultiplePetViewModel model)
		{
			var message = "";
			var isSuccess = false;
			var title = "";

			try
			{
				foreach (var petAppointment in model.PetAppointments)
				{
					if (petAppointment.Patient_ID == 0)
					{
						var pet = new PatientTbl()
						{
							PatientName = petAppointment.PetName,
							Species = petAppointment.Species,
							Breed = petAppointment.Breed,
							ColorMarking = petAppointment.ColorMarking,
							Birthday = petAppointment.Birthday,//x
							Sex = petAppointment.Sex,//x
							Description = petAppointment.Description,
							Owner_ID = petAppointment.Owner_ID,
						};

						await _patientRepo.AddAsync(pet);
						await _patientRepo.SaveChangesAsync();


						var appointment = new AppointmentTbl()
						{
							Service_ID = petAppointment.Service_ID,
							Date = model.Date,
							Time = petAppointment.AppointmentTime,
							Status = "Approved",
							Patient_ID = pet.Patient_ID,
							Owner_ID = petAppointment.Owner_ID,
							IsViewed = true,
						};

						await _appointRepo.AddAsync(appointment);
						await _appointRepo.SaveChangesAsync();
					}
					else
					{
						var appointment = new AppointmentTbl()
						{
							Date = model.Date,
							Time = petAppointment.AppointmentTime,
							Patient_ID = petAppointment.Patient_ID,
							Owner_ID = petAppointment.Owner_ID,
							Status = "Approved",
							Service_ID = petAppointment.Service_ID,
							IsViewed = true,
						};
						await _appointRepo.AddAsync(appointment);
						await _appointRepo.SaveChangesAsync();
					}

				}
			
				

				isSuccess = true;
				message = "Successfully saved!";
				title = "Appointments Added!";

				TempData["ToastSuccessMsg"] = "Appointment successfully added.";

				
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



		public class AppointmentMultipleNewClientPetViewModel
		{
			public DateTime Date { get; set; } // Single date for the appointment
			public string? Firstname { get; set; }
			public string? Middlename { get; set; }
			public string? Lastname { get; set; }
			public string? SuffixName { get; set; }
			public string? Province { get; set; }
			public string? City { get; set; }
			public string? Barangay { get; set; }
			public string? Street { get; set; }
			public string? Phone { get; set; }
			public string? Sex { get; set; }
			public string? Email { get; set; }

			public List<PetAppointmentNewClientViewModel> PetAppointments { get; set; } // List of pets with appointment times
		}

		public class PetAppointmentNewClientViewModel
		{
			public string PetName { get; set; }
			public string Species { get; set; }
			public string? Breed { get; set; }
			public DateTime? Birthday { get; set; }//x
			public string Sex { get; set; }//x
			public string ColorMarking { get; set; }
			public string? Description { get; set; }
			public string AppointmentTime { get; set; }
			public long Service_ID { get; set; }
			public long Patient_ID { get; set; }
			public long Owner_ID { get; set; }
		}

		[HttpPost]
		public async Task<IActionResult> MakeAppointmentNewClient([FromBody] AppointmentMultipleNewClientPetViewModel model)
		{
			var message = "";
			var isSuccess = false;
			var title = "";

			try
			{
				var owner = new OwnerTbl()
				{
					Firstname = model.Firstname,
					Midname = model.Middlename,
					Lastname = model.Lastname,
					SuffixName = model.SuffixName,
					Province = model.Province,
					City = model.City,
					Baranggay = model.Barangay,
					Street = model.Street,
					Phone = model.Phone,
					Email = model.Email,
					Sex = model.Sex,
				};
				await _ownerRepo.AddAsync(owner);
				await _ownerRepo.SaveChangesAsync();

				foreach (var petAppointment in model.PetAppointments)
				{

					var pet = new PatientTbl()
					{
						PatientName = petAppointment.PetName,
						Species = petAppointment.Species,
						Breed = petAppointment.Breed,
						ColorMarking = petAppointment.ColorMarking,
						Birthday = petAppointment.Birthday,//x
						Sex = petAppointment.Sex,//x
						Description = petAppointment.Description,
						Owner_ID = owner.Owner_ID,
					};
					await _patientRepo.AddAsync(pet);
					await _patientRepo.SaveChangesAsync();




					var appointment = new AppointmentTbl()
					{
						Service_ID = petAppointment.Service_ID,
						Date = model.Date,
						Time = petAppointment.AppointmentTime,
						Status = "Approved",
						Patient_ID = pet.Patient_ID,
						Owner_ID = owner.Owner_ID,
						IsViewed = true,
					};
					await _appointRepo.AddAsync(appointment);
					await _appointRepo.SaveChangesAsync();
				}



				isSuccess = true;
				message = "Successfully saved!";
				title = "Appointments Added!";

				TempData["ToastSuccessMsg"] = "Appointment successfully added.";


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
		public async Task<IActionResult> Cancelled(long ID)
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

	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicCapstone.EmailNotication;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Repository;
using NuGet.Protocol;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Authorization;

namespace VetClinicCapstone.Controllers.Doctor
{
    [Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
    public class AppointmentController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IRepository<AppointmentTbl> _appointRepo;
		private readonly IRepository<PatientTbl> _patientRepo;
		private readonly IRepository<OwnerTbl> _ownerRepo;
		private readonly IRepository<ServiceTbl> _serviceRepo;
		private readonly IEmailService _emailService;

		public AppointmentController(ApplicationDbContext context,IRepository<AppointmentTbl> appointRepo, IRepository<ServiceTbl> serviceRepo, IRepository<PatientTbl> patientRepo, IRepository<OwnerTbl> ownerRepo,IEmailService emailService)
		{
			_appointRepo = appointRepo;
			_patientRepo = patientRepo;
			_ownerRepo = ownerRepo;
			_emailService = emailService;
			_serviceRepo = serviceRepo;
			_context = context;
		}
		public IActionResult AppointmentList()
		{
			var appointlist = from appoint in _appointRepo.GetAll()
							  join patient in _patientRepo.GetAll() on appoint.Patient_ID equals patient.Patient_ID
							  join owner in _ownerRepo.GetAll() on patient.Owner_ID equals owner.Owner_ID
							  join service in _serviceRepo.GetAll() on appoint.Service_ID equals service.Service_ID

							  where appoint.Status == "Approved"
							  select new JoinAppointPatientOwner
							  {
								  ServiceTbls = service,
								  AppointmentTbls = appoint,
								  PatientTbls = patient,
								  OwnerTbls = owner

							  };
			return View("../Doctor/Appointment/AppointmentList", appointlist.ToList());
		}
		public IActionResult TodayAppointmentList()
		{
			var appointlist = from appoint in _appointRepo.GetAll()
							  join patient in _patientRepo.GetAll() on appoint.Patient_ID equals patient.Patient_ID
							  join owner in _ownerRepo.GetAll() on patient.Owner_ID equals owner.Owner_ID
							  join service in _serviceRepo.GetAll() on appoint.Service_ID equals service.Service_ID
							  where appoint.Status == "Approved" && appoint.Date == DateTime.Today
							  select new JoinAppointPatientOwner
							  {
								  ServiceTbls = service,
								  AppointmentTbls = appoint,
								  PatientTbls = patient,
								  OwnerTbls = owner

							  };
			return View("../Doctor/Appointment/TodayAppointmentList", appointlist.ToList());
		}

		public IActionResult AppointmentForApproveList()
		{
			var appointlist = from appoint in _appointRepo.GetAll()
							  join patient in _patientRepo.GetAll() on appoint.Patient_ID equals patient.Patient_ID
							  join owner in _ownerRepo.GetAll() on patient.Owner_ID equals owner.Owner_ID
							  join service in _serviceRepo.GetAll() on appoint.Service_ID equals service.Service_ID

							  where appoint.Status == "For Approval" && appoint.IsDeleted ==0
							  select new JoinAppointPatientOwner
							  {
								  AppointmentTbls = appoint,
								  PatientTbls = patient,
								  OwnerTbls = owner,
								   ServiceTbls = service
							  };
			return View("../Doctor/Appointment/ForApprovalList", appointlist.ToList());
		}
		[HttpGet]
		public IActionResult CancelledAppointment()
		{
			var appointlist = from appoint in _appointRepo.GetAll()
							  join patient in _patientRepo.GetAll() on appoint.Patient_ID equals patient.Patient_ID
							  join owner in _ownerRepo.GetAll() on patient.Owner_ID equals owner.Owner_ID
							  join service in _serviceRepo.GetAll() on appoint.Service_ID equals service.Service_ID

							  where appoint.Status == "Cancelled"
							  select new JoinAppointPatientOwner
							  {
								  ServiceTbls = service,
								  AppointmentTbls = appoint,
								  PatientTbls = patient,
								  OwnerTbls = owner

							  };
			return View("../Doctor/Appointment/CancelledAppointment", appointlist.ToList());
		}
		[HttpGet]
		public async Task<IActionResult>CancelAppointment(long ID)
		{
			var appointDetail = await (from appointment in _context.AppointmentTbls
									   join patient in _context.PatientTbls on appointment.Patient_ID equals patient.Patient_ID
									   join owner in _context.OwnerTbls on patient.Owner_ID equals owner.Owner_ID
									   where appointment.Appointment_ID == ID
									   select new
									   {
										   appointment.Appointment_ID,
										   appointment.Date,
										   appointment.Time,
										   owner.Firstname,
										   owner.Email,
										   patient.PatientName
									   }).FirstOrDefaultAsync();
			if (appointDetail == null)
			{
				return NotFound("Appointment not found.");
			}

			if (appointDetail.Email == "")
			{
				var cancelledNullEmail = await _appointRepo.GetByIDAsync(ID);
				if (cancelledNullEmail != null)
				{
					cancelledNullEmail.Status = "Cancelled";
					await _appointRepo.SaveChangesAsync();
					return RedirectToAction("AppointmentList", "Appointment");

				}
			}

			string toEmail = appointDetail.Email;
			string toName = appointDetail.Firstname;
			string petname = appointDetail.PatientName;
			string date = appointDetail.Date.ToString("MMMM dd, yyyy");
			string time = appointDetail.Time;

			string subject = "Your Appointment cancelled";
			string message = $@"
Dear {toName},

We regret to inform you that your appointment scheduled on  {date} at {time} has been cancelled.
If you have any questions or need further assistance, please do not hesitate to contact us at 0915 057 8906.
We apologize for any inconvenience this may cause.



Thank you for your understanding.
Sincerely,


Urban Vet Animal Clinic
Taysan, Batangas


0915 057 8906
urbanvetanimalclinic@gmail.com
urbanvetanimalclinic.com
		      ";

			await _emailService.SendEmailAsync(toEmail, subject, message);

			var cancelled = await _appointRepo.GetByIDAsync(ID);
			if (cancelled != null)
			{
				cancelled.Status = "Cancelled";
				await _appointRepo.SaveChangesAsync();
				return RedirectToAction("AppointmentList", "Appointment");

			}
			return View();
		}
//		[HttpPost]
//		public async Task<IActionResult> SendApprovalEmail(long ID)
//		{
//			// Retrieve owner details by email
//			var getApproval = await _appointRepo.GetFirstOrDefaultAsync(e => e.Appointment_ID == ID);

//			if (getApproval == null)
//			{
//				return NotFound("Appointment not found.");
//			}

//			// Join the tables to get appointment and patient details
//			var appointmentDetails = await (from appointment in _context.AppointmentTbls
//											join patient in _context.PatientTbls on appointment.Patient_ID equals patient.Patient_ID
//											join owner in _context.OwnerTbls on patient.Owner_ID equals owner.Owner_ID
//											where appointment.Appointment_ID == ID
//											select new
//											{
//												appointment.Appointment_ID,
//												appointment.Date,
												
//												ServiceName = "Consultation", // Adjust this if you have a service field
//												owner.Firstname,
//												owner.Email,
//												patient.PatientName
//											}).FirstOrDefaultAsync();

//			if (appointmentDetails == null)
//			{
//				return NotFound("Appointment not found.");
//			}

//			// Extract details for the email
//			string toEmail = appointmentDetails.Email;
//			string toName = appointmentDetails.Firstname;
//			string appointmentDate = appointmentDetails.Date.ToString("MMMM dd, yyyy");
			

//			// Construct email message
//			string subject = "Your Appointment Approved";
//			string message = $@"
//Dear {toName},

//We are pleased to inform you that your appointment has been confirmed. Here are the details of your appointment:

//Appointment Details:

//- Date: {appointmentDate}
//- Service: {appointmentDetails.ServiceName}

//Location:
//Urban Vet Animal Clinic
//Tayasan, Batangas

//Please ensure to arrive at least 10 minutes before your scheduled time. If you need to reschedule or cancel your appointment, please contact us at 09676599591 or urbanvetanimalclinic@gmail.com as soon as possible.

//We look forward to seeing you and assisting you with your healthcare needs.

//Best regards,

//Urban Vet Animal Clinic
//09676599591
//urbanvetanimalclinic@gmail.com
//urvanvetclinic.com
//";

//			// Send email
//			await _emailService.SendEmailAsync(toEmail, subject, message);

//			// Update appointment status
//			var appointmentToUpdate = await _appointRepo.GetByIDAsync(ID);
//			if (appointmentToUpdate != null)
//			{
//				appointmentToUpdate.Status = "Approved";
//				await _appointRepo.SaveChangesAsync();
//			}

//			return RedirectToAction("AppointmentForApproveList");
//		}





		[HttpPost]
		public async Task<IActionResult> SendApprovalEmail(long ID)
		{

			var appointDetail = await (from appointment in _context.AppointmentTbls
											join patient in _context.PatientTbls on appointment.Patient_ID equals patient.Patient_ID
											join owner in _context.OwnerTbls on patient.Owner_ID equals owner.Owner_ID
											where appointment.Appointment_ID == ID
											select new
											{
												appointment.Appointment_ID,
												appointment.Date,
												appointment.Time,
												owner.Firstname,
												owner.Email,
												patient.PatientName
											}).FirstOrDefaultAsync();
			if (appointDetail == null)
			{
				return NotFound("Appointment not found.");
			}

			string toEmail = appointDetail.Email;
			string toName = appointDetail.Firstname;
			string petname = appointDetail.PatientName;
			string date = appointDetail.Date.ToString("MMMM dd, yyyy");
			string time = appointDetail.Time;

			string subject = "Your Appointment approved";
			string message = $@"
Dear {toName},

We are pleased to inform you that your appointment has been confirmed. Here are the details of your appointment:

Appointment Details:

- Date: {date}
- Time: {time}
- Pet Name : {petname}

Location:
Urban Vet Animal Clinic
Taysan, Batangas

Please ensure to arrive at least 10 minutes before your scheduled time. If you need to reschedule or cancel your appointment, please reschedule your appointment at urbanvetanimalclinic.com as soon as possible.

We look forward to seeing you and assisting you with your pet healthcare needs.

Best regards,

0915 057 8906
urbanvetanimalclinic@gmail.com
urbanvetanimalclinic.com
		      ";


			await _emailService.SendEmailAsync(toEmail, subject, message);

			var del = await _appointRepo.GetByIDAsync(ID);
			if (del != null)
			{
				del.Status = "Approved";
				await _appointRepo.SaveChangesAsync();
			}

			return RedirectToAction("AppointmentForApproveList");
		}
		public JsonResult GetNewAppointmentCount()
		{
			var count = _context.AppointmentTbls
				.Where(a => a.Status == "For Approval" && !a.IsViewed)
				.Count();

			return Json(new { count });
		}

		public JsonResult MarkAppointmentsAsViewed()
		{
			var appointments = _context.AppointmentTbls
				.Where(a => a.Status == "For Approval" && !a.IsViewed)
				.ToList();

			foreach (var appointment in appointments)
			{
				appointment.IsViewed = true;
			}

			_context.SaveChanges();

			return Json(new { success = true });
		}
		[HttpGet]
		public async Task<IActionResult> Redirection(long appointmentId, long ownerId)
		{
		
			var appointment = await _appointRepo.GetByIDAsync(appointmentId);

			if (appointment != null)
			{
				
				appointment.Status = "Completed"; 
				_appointRepo.Update(appointment);
				await _appointRepo.SaveChangesAsync();

			
				return RedirectToAction("ViewRecord", "Patient", new { id = ownerId });
			}


			return NotFound();
		}
		//[HttpPost]
		//public async Task<IActionResult> Redirection(long ID)
		//{
		//	var getOwnerID = await _ownerRepo.GetByIDAsync(ID);
		//	if (getOwnerID != null)
		//	{
		//		return RedirectToAction("ViewRecord", "Patient", new { id = ID });
		//	}

		//	return Ok();
		//}
	}
}

using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using NuGet.Protocol.Plugins;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using VetClinicCapstone.Controllers.Services;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;
using VetClinicCapstone.Repository;



namespace VetClinicCapstone.Controllers.Doctor
{
	[Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
	public class PatientController : Controller
	{

		private readonly IRepository<PatientTbl> _patientRepo;
		private readonly IRepository<OwnerTbl> _ownerRepo;
		private readonly IRepository<ConsultationTbl> _consultRepo;
		private readonly IRepository<PrescriptionTbl> _presRepo;
		private readonly IRepository<PrescriptionDetailTbl> _presdetailRepo;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IRepository<LaboratoryTbl> _labRepo;
		private readonly ApplicationDbContext _context;
		public PatientController(IRepository<PatientTbl> patientRepo, IRepository<LaboratoryTbl> labRepo, IRepository<PrescriptionDetailTbl> presdetailRepo, IRepository<ConsultationTbl> consultation, IRepository<PrescriptionTbl> presRepo, IRepository<OwnerTbl> ownerRepo, ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
		{
			_patientRepo = patientRepo;
			_ownerRepo = ownerRepo;
			_context = context;
			_consultRepo = consultation;
			_presRepo = presRepo;
			_presdetailRepo = presdetailRepo;
			_labRepo = labRepo;
			_hostingEnvironment = hostingEnvironment;
		}
		//public IActionResult List()
		//{
		//	int rowCount = _context.PatientTbls.Where(i => i.Status == "Walk In").Count();
		//	ViewBag.TotalPatient = rowCount;

		//	var Owners = (from OwnerTbls in _context.OwnerTbls
		//				  join UserTbls in _context.UserTbls on OwnerTbls.Owner_ID equals UserTbls.Owner_ID into userGroup
		//				  from user in userGroup.DefaultIfEmpty()
		//				  where _context.PatientTbls.Any(p => p.Owner_ID == OwnerTbls.Owner_ID)
		//				  select new UserAndOwnerViewModel
		//				  {
		//					  OwnerDetail = OwnerTbls,
		//					  UserDetail = user
		//				  }
		//	).ToList();

		//	return View("../Doctor/Patient/List", Owners);
		//}
		public IActionResult List()
		{
			int rowCount = _context.PatientTbls.Where(i => i.Status == "Walk In").Count();
			ViewBag.TotalPatient = rowCount;

            var Owners = (from owner in _context.OwnerTbls
                          join user in _context.UserTbls on owner.Owner_ID equals user.Owner_ID into userGroup
                          from user in userGroup.DefaultIfEmpty()
                          where user == null || user.IsActivated == 1
                          orderby owner.Owner_ID descending // Arranging in descending order by Owner_ID
                          select new UserAndOwnerViewModel
                          {
                              OwnerDetail = owner,
                              UserDetail = user
                          }).ToList();

            //var Owners = (from OwnerTbls in _context.OwnerTbls
            //			  join UserTbls in _context.UserTbls on OwnerTbls.Owner_ID equals UserTbls.Owner_ID into userGroup
            //			  from user in userGroup.DefaultIfEmpty()
            //			  where (user == null || user.IsActivated == 1) 
            //					//_context.PatientTbls.Any(p => p.Owner_ID == OwnerTbls.Owner_ID)
            //			  select new UserAndOwnerViewModel
            //			  {
            //				  OwnerDetail = OwnerTbls,
            //				  UserDetail = user
            //			  }
            //).ToList();

            return View("../Doctor/Patient/List", Owners);
		}
		[HttpGet]
		public IActionResult AddPatientInfo()
		{
			return View("../Doctor/Patient/AddPatientInfo");
		}
		[HttpPost]
		public async Task<IActionResult> AddPatientInfo(PatientTbl model)
		{
			var isSuccess = false;
			var message = "";
			var title = "";
			try
			{
				var addpet = new PatientTbl
				{
					PatientName = model.PatientName,
					Breed = model.Breed,
					Species = model.Species,
					Status = model.Status,
					Description = model.Description,
					ColorMarking = model.ColorMarking,
					Sex = model.Sex,
					Birthday = model.Birthday,
					Owner_ID = model.Owner_ID
				};
				await _patientRepo.AddAsync(addpet);
				await _patientRepo.SaveChangesAsync();

				isSuccess = true;
				message = "Successfully saved!";
				title = "Pet Added!";
			}
			catch (Exception ex)
			{

				message = ex.Message;
			}


			return Json(new { success = isSuccess, message = message, title = title });

		}
		[HttpGet]
		public async Task<IActionResult> ViewRecord(long ID)
		{
			try
			{
				var ownerWithPets = new JoinPatientOwnerViewModel
				{
					OwnerTbls = await _context.OwnerTbls.FirstOrDefaultAsync(o => o.Owner_ID == ID),
					PatientTbls = await _context.PatientTbls.FirstOrDefaultAsync(o => o.Owner_ID == ID),
					PatientTblsList = await _context.PatientTbls.Where(p => p.Owner_ID == ID &&p.IsDeleted==0).ToListAsync(),///xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
					ConsultationWithDoctorViewModels = await (from consult in _context.ConsultationTbls
															  join doctor in _context.AdminInfoTbls on consult.AdminInfo_ID equals doctor.AdminInfo_ID
															  join patient in _context.PatientTbls on consult.Patient_ID equals patient.Patient_ID
															  join service in _context.ServiceTbls on consult.Service_ID equals service.Service_ID

															  select new ConsultationWithDoctorViewModel
															  {
																  PatientTbls = patient,
																  ServiceTbls = service,
																  DoctorTbls = doctor,
																  ConsultationTbls = consult
															  }).ToListAsync(),



					PrescriptionWithDoctorViewModels = await _context.PrescriptionTbls
	.FromSqlRaw(@"SELECT p.*
                       FROM PrescriptionTbl p
                       INNER JOIN AdminInfoTbl a ON p.AdminInfo_ID = a.AdminInfo_ID")
	.Select(p => new PrescriptionWithDoctorViewModel
	{
		PrescriptionTbls = p,
		AdminInfoTbls = p.AdminInfo,
	})
	.ToListAsync(),


					LaboratoryTbls = await _context.LaboratoryTbls.ToListAsync(),
					TreatmentTbls = await _context.TreatmentTbls.ToListAsync(),
					ServiceTbls = await _context.ServiceTbls.Where(a=>a.IsDeleted == 0).ToListAsync(),

				};

				return View("../Doctor/Patient/ViewRecord", ownerWithPets);
			}
			catch (Exception)
			{

				throw;
			}

		}

		//public async Task<IActionResult> MedicalInformation(long ID)
		//{
		//	var consultations = await _context.ConsultationTbls
		//		.FromSqlRaw("SELECT * FROM ConsultationTbl WHERE Consultation_ID = {0}", ID)
		//		.FirstOrDefaultAsync();

		//	if (consultations == null)
		//	{
		//		return View("Consultation not found!");
		//	}

		//	var consultationList = await _context.ConsultationTbls.Where(i => i.Consultation_ID == ID).ToListAsync();
		//	var lab = await _context.LaboratoryTbls.Where(i => i.Consultation_ID == ID).ToListAsync();

		//	var prescriptionsWithDoctors = await _context.PrescriptionTbls
		//		.FromSqlRaw(@"SELECT p.*
  //                     FROM PrescriptionTbl p
  //                     INNER JOIN AdminInfoTbl a ON p.AdminInfo_ID = a.AdminInfo_ID
  //                     WHERE p.Consultation_ID = {0}", ID)
		//		.Select(p => new PrescriptionWithDoctorViewModel
		//		{
		//			PrescriptionTbls = p,
		//			AdminInfoTbls = p.AdminInfo,
		//		})
		//		.ToListAsync();

		//	var result = new PetConsultViewModel
		//	{
		//		LaboratoryTbls = lab,
		//		ConsultationTblsList = consultationList,
		//		ConsultationTbls = consultations,
		//		PrescriptionWithDoctorViewModels = prescriptionsWithDoctors
		//	};

		//	return View("../Doctor/Patient/MedicalInformation", result);
		//}
		//[HttpGet]
		//public async Task<IActionResult> ViewDetail(long ID)

		//{
		//	var patient = await _context.PatientTbls.FirstOrDefaultAsync(o => o.Patient_ID == ID);


		//	var consultations = await (from c in _context.ConsultationTbls
		//							   join d in _context.AdminInfoTbls on c.AdminInfo_ID equals d.AdminInfo_ID
		//							   where c.Patient_ID == ID
		//							   select new ConsultationWithDoctorViewModel
		//							   {

		//								   DoctorTbls = d,
		//								   ConsultationTbls = c
		//							   }).ToListAsync();

		//	//var consultation = await _context.ConsultationTbls
		//	//						 .Where(p => p.Patient_ID == ID)
		//	//						 .ToListAsync();

		//	var result = new PetConsultViewModel
		//	{
		//		PatientTbls = patient,
		//		ConsultationWithDoctorViewModels = consultations
		//	};

		//	return View("../Doctor/Patient/ViewDetail", result);

		//}

		[HttpGet]
		public IActionResult AddOwnerInfo()
		{
			return View("../Doctor/Patient/AddOwnerInfo");
		}
		[HttpPost]
		public async Task<IActionResult> AddOwnerInfo(OwnerTbl model)
		{
			var message = "";
			var isSuccess = false;
			var title = "";
			try
			{
				var owner = new OwnerTbl()
				{
					Firstname = model.Firstname,
					Midname = model.Midname,
					Lastname = model.Lastname,
					Email = model.Email,
					Phone = model.Phone,
					Province = model.Province,
					City = model.City,
					Baranggay = model.Baranggay,
					Street = model.Street,
					Sex = model.Sex,
				};
				await _ownerRepo.AddOwnerAsync(owner);
				await _ownerRepo.SaveChangesAsync();

				isSuccess = true;
				message = "Successfully saved!";
				title = "Owner Added!";

			}
			catch (Exception ex)
			{

				message = ex.Message;
			}
			return Json(new { success = isSuccess, message = message, title = title });
		}

		[HttpPost]
		public async Task<IActionResult> AddConsultation(ConsultationTbl model,PatientTbl PatientModel, List<string> Laboratories)
		{

			var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var isSuccess = false;
			var message = "";
			var title = "";
			try
			{
				var AddConsult = new ConsultationTbl()
				{
					Service_ID = model.Service_ID,
					Weight = model.Weight,
					Temparature = model.Temparature,
					RepiratoryRate = model.RepiratoryRate,
					HeartRate = model.HeartRate,
					ClinicalSign = model.ClinicalSign,
					Diagnosis = model.Diagnosis,
					Patient_ID = model.Patient_ID,
					ConsultationDate = DateTime.Now,
					AdminInfo_ID = Convert.ToInt32(AdminID)

				};
				await _consultRepo.AddAsync(AddConsult);
				await _consultRepo.SaveChangesAsync();


				if (Laboratories != null && Laboratories.Count > 0)
				{
					foreach (var lab in Laboratories)
					{
						var addLabo = new LaboratoryTbl
						{
							LaboratoryName = lab,
							Consultation_ID = AddConsult.Consultation_ID


						};
						await _labRepo.AddAsync(addLabo);
					}
					await _labRepo.SaveChangesAsync();
				}

				isSuccess = true;
				message = "Successfully saved!";
				title = "Consulation Added!";
			}
			catch (Exception ex)
			{

				message = ex.Message;
			}


			return Json(new { success = isSuccess, message = message, title = title });
		}

		[HttpGet]
		public async Task<IActionResult> UpdateOwnerInfo(long ID)
		{
			var getOwnerInfo = await _ownerRepo.GetByIDAsync(ID);


			if (getOwnerInfo != null)
			{
				var owner = new OwnerTbl()
				{
					Owner_ID = getOwnerInfo.Owner_ID,
					Firstname = getOwnerInfo.Firstname,
					Midname = getOwnerInfo.Midname,
					SuffixName = getOwnerInfo.SuffixName,
					Province = getOwnerInfo.Province,
					City = getOwnerInfo.City,
					Baranggay = getOwnerInfo.Baranggay,
					Street = getOwnerInfo.Street,
					Phone = getOwnerInfo.Phone,
					Email = getOwnerInfo.Email,
					Sex = getOwnerInfo.Sex,

				};
				return PartialView("../Doctor/Patient/_EditOwnerInfoPartialView.cshtml", getOwnerInfo);
			}
			return View("../Doctor/Patient/List.cshtml", getOwnerInfo);
		}
		[HttpPost]
		public async Task<IActionResult> EditOwnerInfo(OwnerTbl model)
		{
			var isSuccess = false;
			var message = "";
			var title = "";
			try
			{
				var getowner = await _ownerRepo.GetByIDAsync(model.Owner_ID);
				if (getowner != null)
				{
					getowner.Firstname = model.Firstname;
					//getowner.Midname = model.Midname;
					getowner.Lastname = model.Lastname;
					//getowner.SuffixName = model.SuffixName;
					getowner.Province = model.Province;
					getowner.City = model.City;
					getowner.Baranggay = model.Baranggay;
					//getowner.Street = model.Street;
					getowner.Phone = model.Phone;
					getowner.Email = model.Email;
					//getowner.Sex = model.Sex;

					_ownerRepo.Update(getowner);
					await _ownerRepo.SaveChangesAsync();

					isSuccess = true;
					message = "Successfully saved!";
					title = "Owner Info Updated!";
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}

			return Json(new { success = isSuccess, message = message, title = title });
		}
		//[HttpGet]
		//public async Task<IActionResult> UpdatePatientInfo(long ID)
		//{
		//	var getPetInfo = await _patientRepo.GetByIDAsync(ID);


		//	if (getPetInfo != null)
		//	{
		//		var pet = new PatientTbl()
		//		{
		//			Patient_ID = getPetInfo.Patient_ID,
		//			PatientName = getPetInfo.PatientName,
		//			Breed = getPetInfo.Breed,
		//			Species = getPetInfo.Species,
		//			Description = getPetInfo.Description,
		//			Sex = getPetInfo.Sex,
		//			ColorMarking = getPetInfo.ColorMarking,
		//			Birthday = getPetInfo.Birthday,

		//		};
		//		return PartialView("../Doctor/Patient/_EditPatientInfoPartialView.cshtml", getPetInfo);
		//	}
		//	return View("../Doctor/Patient/ViewRecord.cshtml", getPetInfo);
		//}
		[HttpPost]
		public async Task<IActionResult> EditPatientInfo(PatientTbl model)
		{
			var isSuccess = false;
			var message = "";
			var title = "";
			try
			{
				var getPetinfo = await _patientRepo.GetByIDAsync(model.Patient_ID);
				if (getPetinfo != null)
				{
					getPetinfo.PatientName = model.PatientName;
					getPetinfo.Breed = model.Breed;
					getPetinfo.Species = model.Species;
					getPetinfo.Description = model.Description;
					getPetinfo.Sex = model.Sex;
					getPetinfo.ColorMarking = model.ColorMarking;
					getPetinfo.Birthday = model.Birthday;

					_patientRepo.Update(getPetinfo);
					await _patientRepo.SaveChangesAsync();

					isSuccess = true;
					message = "Successfully saved!";
					title = "Patient Info Updated!";
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
			}

			return Json(new { success = isSuccess, message = message, title = title });
		}
		public class SavePrescriptionsRequest
		{
            public long Patient_ID { get; set; } //xx
            public long Treatment_ID { get; set; }
            public long Consultation_ID { get; set; }
			public string Disease { get; set; }
			public List<PrescriptionDetailViewModel> Prescriptions { get; set; }
		}

		public class PrescriptionDetailViewModel
		{
			public string Medicine { get; set; }
			public string Dosage { get; set; }
			public string Frequency { get; set; }
			public string Instruction { get; set; }
		}
		[HttpPost("SavePrescriptions")]
		public async Task<IActionResult> SavePrescriptions([FromBody] SavePrescriptionsRequest request)
		{
			var AdminID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var addprescript = new PrescriptionTbl()
			{
                Patient_ID = request.Patient_ID, 
                Consultation_ID = request.Consultation_ID,
				AdminInfo_ID = Convert.ToInt32(AdminID),
				DatePrescribed = DateTime.Now,

			};

			await _context.PrescriptionTbls.AddAsync(addprescript);
			await _context.SaveChangesAsync();



			var getTreatmentID = await _context.TreatmentTbls.FindAsync(request.Treatment_ID);

			if (getTreatmentID !=null)
			{
				getTreatmentID.Prescription_ID = addprescript.Prescription_ID;

                _context.TreatmentTbls.Update(getTreatmentID);
                await _context.SaveChangesAsync();
            }



            var newPrescriptionID = addprescript.Prescription_ID;

			if (request.Prescriptions == null || !request.Prescriptions.Any())
			{
				return BadRequest("No prescriptions provided.");
			}

			foreach (var prescription in request.Prescriptions)
			{
				var prescriptionDetail = new PrescriptionDetailTbl
				{
					Medicine = prescription.Medicine,
					Dosage = prescription.Dosage,
					Frequency = prescription.Frequency,
					Instruction = prescription.Instruction,
					Prescription_ID = newPrescriptionID
				};

				await _context.PrescriptionDetailTbls.AddAsync(prescriptionDetail);
			}

			await _context.SaveChangesAsync();

            return Ok(new{newId = newPrescriptionID});
        }

        //public IActionResult PrescriptionDetails(long ID)
        //{
        //	var presDetail = _presdetailRepo.GetAll().Where(i => i.Prescription_ID == ID);
        //	return View("../Doctor/Patient/PrescriptionDetails", presDetail);
        //}
        public IActionResult PrescriptionDetails(long ID)
        {
            var prescription = (from p in _context.PrescriptionTbls
                                join pt in _context.PatientTbls on p.Patient_ID equals pt.Patient_ID
                                join a in _context.AdminInfoTbls on p.AdminInfo_ID equals a.AdminInfo_ID
                                where p.Prescription_ID == ID
                                select new
                                {
                                    PrescriptionTbls = p,
                                    PatientTbls = pt,
                                    AdminInfoTbls = a
                                }).FirstOrDefault();

            if (prescription == null)
            {
                return NotFound();
            }

        
            var prescriptionDetails = _context.PrescriptionDetailTbls
                .Where(pd => pd.Prescription_ID == ID)
                .ToList();

            var model = new PrescriptionWithDoctorViewModel
            {
                PrescriptionTbls = prescription.PrescriptionTbls,
                PatientTbls = prescription.PatientTbls,
                AdminInfoTbls = prescription.AdminInfoTbls,
                PrescriptionDetailTbls = prescriptionDetails
            };

            return View("../Doctor/Patient/PrescriptionDetails", model);
        }


        [HttpPost]
		public async Task<IActionResult> ImageUpload(IFormFile imageFile, long id)
		{
			if (imageFile == null || imageFile.Length == 0)
			{
				return RedirectToAction("Index");
			}

			string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "PetImage");
			if (!Directory.Exists(uploadsFolder))
			{
				Directory.CreateDirectory(uploadsFolder);
			}

			string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
			string filePath = Path.Combine(uploadsFolder, uniqueFileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await imageFile.CopyToAsync(stream);
			}

			var existingPatient = await _patientRepo.GetByIDAsync(id);
			if (existingPatient == null)
			{
				return NotFound();
			}

			existingPatient.FileName = uniqueFileName;

			_patientRepo.Update(existingPatient);
			await _patientRepo.SaveChangesAsync();

			return Ok();
		}


        [HttpPost]
        public async Task<IActionResult> LaboratoryUpload(IFormFile labResultUpload, LaboratoryTbl model)
        {
			var title = "";
			var message = "";
			var isSuccess = false;
			try
			{
                if (labResultUpload == null || labResultUpload.Length == 0)
                {
                    return RedirectToAction("Index");
                }

                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "LabotatoryResultImage");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(labResultUpload.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await labResultUpload.CopyToAsync(stream);
                }

                var existingLab = await _labRepo.GetByIDAsync(model.Laboratory_ID);
                if (existingLab == null)
                {
                    return NotFound();
                }

                existingLab.LabFileName = uniqueFileName;

                _labRepo.Update(existingLab);
                await _labRepo.SaveChangesAsync();

                isSuccess = true;
                message = "Successfully uploaded!";
                title = "Lab result uploaded!";

            }
            catch (Exception ex)
			{

				message = ex.Message;
			}
           
            return Json(new {title = title, message = message, success=isSuccess });
        }
        [HttpPost]
		public async Task<IActionResult> UpdateDiagnosis(ConsultationTbl model)
		{
            var isSuccess = false;
            var message = "";
            var title = "";
            string newDiagnosis = "";
            try
            {

				var consultation = await _consultRepo.GetByIDAsync(model.Consultation_ID);

				if (consultation != null)
				{
					consultation.Diagnosis = model.Diagnosis;

					_consultRepo.Update(consultation);

					await _consultRepo.SaveChangesAsync();

					isSuccess = true;
					message = "Successfully saved!";
					title = "Diagnosis Updated!";
                    newDiagnosis = consultation.Diagnosis;
                }

			}
			catch (Exception ex)
			{
				message = ex.Message;
			}

			return Json(new { success = isSuccess, message = message, title = title, newDiagnosis = newDiagnosis });
		}
        [HttpPost]
        public async Task<IActionResult> AddTreatment(TreatmentTbl model)
        {
            var message = "";
            var isSuccess = false;
            var title = "";
            try
            {
                var addTreatment = new TreatmentTbl()
                {
                    TreatmentType = model.TreatmentType,
                    Notes = model.Notes,
                    FollowUpCheckUp = model.FollowUpCheckUp,
                    Consultation_ID = model.Consultation_ID,
                };
				await _context.TreatmentTbls.AddAsync(addTreatment);
				await _context.SaveChangesAsync();

				isSuccess = true;
                message = "Successfully saved!";
                title = "Treatment Added!";

                // Return the newly added treatment details
                return Json(new
                {
                    success = isSuccess,
                    message = message,
                    title = title,
                    treatmentType = addTreatment.TreatmentType,
                    notes = addTreatment.Notes,
                    followUpCheckUp = addTreatment.FollowUpCheckUp?.ToString("MMMM dd, yyyy"),
                    consultationId = addTreatment.Consultation_ID,
                    treatmentId = addTreatment.Treatment_ID
                });
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { success = isSuccess, message = message, title = title });
        }
		[HttpPost]
		public async Task<IActionResult> DeletePetsList(long ID)
		{
			var IsSuccess = false;
			var title = "";
			var message = "";

			try
			{
				var getId = await _patientRepo.GetByIDAsync(ID);

				if (getId != null)
				{
					getId.IsDeleted = 1;

					_patientRepo.Update(getId);
					await _patientRepo.SaveChangesAsync();

					IsSuccess = true;
					title = "Success";
					message = "The pet has been successfully deleted.";
				}	


			}
			catch (Exception)
			{

				throw;
			}
			return Json (new { title=title,message=message,success=IsSuccess});
		}

	}
}
	













		






//[HttpPost]
//public async Task<IActionResult> AddPatient(AddPatientOwnerViewModel model)
//{

//	var message = "";
//	var isSuccess = false;
//	var title = "";
//	try
//	{

//		var owner = new OwnerTbl()
//		{
//			Guid_id = Guid.NewGuid(),
//			Firstname = model.OwnerDetail.Firstname,
//			Midname = model.OwnerDetail.Midname,
//			Lastname = model.OwnerDetail.Lastname,
//			Email = model.OwnerDetail.Email,
//			Phone = model.OwnerDetail.Phone,
//			Province = model.OwnerDetail.Province,
//			City = model.OwnerDetail.Email,
//			Baranggay = model.OwnerDetail.Baranggay,
//		};
//		await _ownerRepo.AddOwnerAsync(owner);
//		await _ownerRepo.SaveChangesAsync();


//		var pet = new PatientTbl()
//		{
//			Guid_id = Guid.NewGuid(),
//			PatientName = model.PatientDetail.PatientName,
//			Species = model.PatientDetail.Species,
//			Breed = model.PatientDetail.Breed,
//			Status = model.PatientDetail.Status = "Walk In",
//			Owner_ID = owner.Owner_ID,
//		};


//		await _patientRepo.AddAsync(pet);
//		await _patientRepo.SaveChangesAsync();


//		isSuccess = true;
//		message = "Successfully saved!";
//		title = "Patient Added!";
//	}
//	catch (Exception ex)
//	{
//		message = ex.Message;
//		// Handle exception appropriately
//		// You might want to log the exception or return a different response based on your application's requirements
//	}

//	return Json(new { success = isSuccess, message = message , title = title });
//}
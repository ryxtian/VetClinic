using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VetClinicCapstone.Models;
using VetClinicCapstone.Models.ViewModel;

namespace VetClinicCapstone.Controllers.UserPatient
{
    [Authorize(Policy = "UserOnly", AuthenticationSchemes = "UserScheme")]
    public class UserPatient : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public UserPatient(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(owner, out int ownerId))
            {
                var query = from patient in _context.PatientTbls
                            where patient.Owner_ID == ownerId && patient.IsDeleted ==0  //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            select new AddPatientOwnerViewModel
                            {
                                PatientDetail = patient,
                                ConsultationDetail = _context.ConsultationTbls
                                    .Where(c => c.Patient_ID == patient.Patient_ID)
                                    .OrderBy(c => c.ConsultationDate)
                                    .FirstOrDefault()
                            };

                var model = await query.ToListAsync();
                return View("../User/Patient/Index", model);
            }

            return BadRequest("Invalid Owner ID");
        }

        public async Task<IActionResult> AddPet(PatientTbl model)
        {
            var owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var message = "";
            var title = "";
            var isSuccess = false;

            try
            {
                var addpet = new PatientTbl()
                {
                    PatientName = model.PatientName,
                    Species = model.Species,
                    Breed = model.Breed,
                    ColorMarking = model.ColorMarking,
                    Birthday = model.Birthday,
                    Sex = model.Sex,
                    Description = model.Description,
                    Owner_ID =Convert.ToInt32(owner),
                };


                await _context.PatientTbls.AddAsync(addpet);
                await _context.SaveChangesAsync();

                isSuccess = true;
                message = "Successfully saved!";
                title = "Pet Added!";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new {title=title,success=isSuccess,message=message });
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePet(PatientTbl model, IFormFile petImage)
        {
            var message = "";
            var title = "";
            var isSuccess = false;

            try
            {
                var getpatient = await _context.PatientTbls.FindAsync(model.Patient_ID);
                if (getpatient != null)
                {
                    bool isNameUpdated = false;
                    bool isImageUpdated = false;

                  
                    if (!string.IsNullOrEmpty(model.PatientName))
                    {
                        getpatient.PatientName = model.PatientName;
                        isNameUpdated = true;
                    }

            
                    if (petImage != null && petImage.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "PetImage");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(petImage.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await petImage.CopyToAsync(stream);
                        }

                        getpatient.FileName = uniqueFileName;
                        isImageUpdated = true;
                    }

                    if (isNameUpdated || isImageUpdated)
                    {
                        _context.PatientTbls.Update(getpatient);
                        await _context.SaveChangesAsync();

                        isSuccess = true;
                        message = "Successfully updated!";
                        title = "Pet Updated!";
                    }
                    else
                    {
                        message = "No changes were made.";
                        title = "Update Failed!";
                    }
                }
                else
                {
                    message = "Patient not found.";
                    title = "Update Failed!";
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                message = "An error occurred: " + ex.Message;
                title = "Update Failed!";
            }

            return Json(new { title = title, message = message, success = isSuccess });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicCapstone.Models;
using VetClinicCapstone.Repository;

namespace VetClinicCapstone.Controllers
{
	[Authorize(AuthenticationSchemes = "AdminScheme")]

	public class AdminDashboard : Controller
    {
		private readonly IRepository<PatientTbl> _patientRepo;
		private readonly ApplicationDbContext _context;
		public AdminDashboard(ApplicationDbContext context, IRepository<PatientTbl> patientRepo)
		{

			_context = context;
			_patientRepo = patientRepo;
		}
		public async Task<IActionResult> Index()
        {

			int rowCount = await _context.ConsultationTbls
			.Where(d => d.ConsultationDate == DateTime.Today)
			.CountAsync();

            int Apppoitnemtn = await _context.AppointmentTbls
.Where(d => d.Date == DateTime.Today && d.Status == "Approved")
.CountAsync();


            ViewBag.Appointment = Apppoitnemtn;
            ViewBag.TotalPatientToday = rowCount;


			//var topSpecies = await _context.PatientTbls
			//		  .GroupBy(p => p.Species)
			//		  .Select(g => new
			//		  {
			//			  Species = g.Key,
			//			  Count = g.Count()
			//		  })
			//		  .OrderByDescending(x => x.Count)
			//		  .Take(3)
			//		  .ToListAsync();



			var topSpecies = await _context.PatientTbls
.Join(_context.ConsultationTbls, // Join with ConsultationTbls
	p => p.Patient_ID,             // Assuming PatientId is the key in PatientTbls
	c => c.Patient_ID,             // and it relates to PatientId in ConsultationTbls
	(p, c) => p)                  // Select patient data after the join
.GroupBy(p => p.Species)           // Group by Species
.Select(g => new
{
	Species = g.Key,
	Count = g.Count()              // Count the number of patients for each species
})
.OrderByDescending(x => x.Count)   // Order by Count descending
.Take(3)                           // Take the top 3 species
.ToListAsync();



			//		var topSpecies = await _context.PatientTbls
			//.Join(_context.ConsultationTbls,
			//	p => p.Patient_ID,  // Join on Patient_ID in both tables
			//	c => c.Patient_ID,
			//	(p, c) => new { p.Patient_ID, p.Species }) // Select only relevant fields
			//.Distinct()  // Ensure that each patient is counted only once
			//.GroupBy(p => p.Species)  // Group by Species
			//.Select(g => new
			//{
			//	Species = g.Key,
			//	Count = g.Count()  // Count distinct patients for each species
			//})
			//.OrderByDescending(x => x.Count)  // Order by Count in descending order
			//.Take(3)  // Take the top 3 species
			//.ToListAsync();

			return View("../Dashboard/DoctorDashboard", topSpecies);
        }


        public class IllnessData
        {
            public string IllnessName { get; set; }
            public int CaseCount { get; set; }
        }
        [HttpGet]
		public async Task<JsonResult> GetIllnessData()
		{
			var data = await _context.ConsultationTbls
				.GroupBy(c => c.Diagnosis)
				.Where(group => group.Key != "N/A")
				.Select(groupedData => new IllnessData
				{
					IllnessName = groupedData.Key,
					CaseCount = groupedData.Count()
				})
				//.OrderByDescending(illness => illness.CaseCount) // Sort by case count in descending order
				.Take(10) 
				.ToListAsync();

			return Json(data);
		}
	}
}

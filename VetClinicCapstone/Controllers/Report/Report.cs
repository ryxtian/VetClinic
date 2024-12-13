using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicCapstone.Models;
using VetClinicCapstone.ReportServices;

namespace VetClinicCapstone.Controllers.Report
{
	[Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]

	public class Report : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly ReportService _reportService;
        public Report(ApplicationDbContext context, ReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }
        public IActionResult Index()
		{
			return View("../Report/Index");
		}
        [HttpGet]
        public async Task<IActionResult> GenerateReport(string reportType, DateTime? startDate, DateTime? endDate)
        
        {
            if (startDate > endDate || endDate<startDate)
            {
                TempData["InvalidDateReport"] = "Start date cannot be greater than end date.";
   
            }
            if (string.IsNullOrEmpty(reportType))
            {
                return BadRequest("Report type is required");
            }

            var report = await _reportService.GenerateReport(reportType, startDate, endDate);

            return View("../Report/Index", report);
        }

        public async Task<IActionResult> GenerateAppointmentReport(string reportType, DateTime? startDate, DateTime? endDate)

        {
            if (startDate > endDate || endDate < startDate)
            {
                TempData["InvalidDateReport"] = "Start date cannot be greater than end date.";

            }
            if (string.IsNullOrEmpty(reportType))
            {
                return BadRequest("Report type is required");
            }

            var report = await _reportService.GenerateAppointmentReport(reportType, startDate, endDate);

            return View("../Report/Appointment", report);
        }

		public async Task<IActionResult> GenerateConsultationReport(string reportType, DateTime? startDate, DateTime? endDate)

		{
			if (startDate > endDate || endDate < startDate)
			{
				TempData["InvalidDateReport"] = "Start date cannot be greater than end date.";

			}
			if (string.IsNullOrEmpty(reportType))
			{
				return BadRequest("Report type is required");
			}

			var report = await _reportService.GenerateConsultationReport(reportType, startDate, endDate);

			return View("../Report/Consultation", report);
		}
		//[HttpGet]
		//public async Task<IActionResult> GenerateReport(string reportType)
		//{
		//    if (string.IsNullOrEmpty(reportType))
		//    {
		//        return BadRequest("Report type is required");
		//    }

		//    var report = await _reportService.GenerateReport(reportType);
		//    return View("../Report/Index",report);
		//}
	}
}

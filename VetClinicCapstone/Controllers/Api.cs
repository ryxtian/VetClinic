using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VetClinicCapstone.Models;

namespace VetClinicCapstone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Api : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public Api(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet("getAppointments")]
		public async Task<IActionResult> GetAppointments()
		{
			try
			{
                var appointments = await _context.AppointmentTbls
                    .Where(s=>s.Status =="Approved")
                    .GroupBy(a => a.Date)
                    .Select(g => new
                    {
                        date = g.Key,
                        count = g.Count()
                    })
                    .ToListAsync();

                return Ok(appointments);
            }
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}

		[HttpGet("MonthlyCount")]
		public IActionResult GetMonthlyCount()
		{
			var data = new List<MonthlyCountDto>
		{
			new MonthlyCountDto { Month = "January", Count = 33 },
			new MonthlyCountDto { Month = "February", Count = 25 },
			new MonthlyCountDto { Month = "March", Count = 35 },
			new MonthlyCountDto { Month = "April", Count = 40 },
			new MonthlyCountDto { Month = "May", Count = 20 },
			new MonthlyCountDto { Month = "June", Count = 45 },
			new MonthlyCountDto { Month = "July", Count = 50 },
			new MonthlyCountDto { Month = "August", Count = 30 },
			new MonthlyCountDto { Month = "September", Count = 35 },
			new MonthlyCountDto { Month = "October", Count = 40 },
			new MonthlyCountDto { Month = "November", Count = 25 },
			new MonthlyCountDto { Month = "December", Count = 30 }
		};

			return Ok(data);
		}
		public class MonthlyCountDto
		{
			public string? Month { get; set; }
			public int Count { get; set; }
		}

	}
}

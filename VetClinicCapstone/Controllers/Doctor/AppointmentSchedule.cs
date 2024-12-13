using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicCapstone.Models;

namespace VetClinicCapstone.Controllers.Doctor
{
    [Authorize(Policy = "DoctorOrStaffOnly", AuthenticationSchemes = "AdminScheme")]
    public class AppointmentSchedule : Controller
    {
        private readonly ApplicationDbContext _context;
        public AppointmentSchedule(ApplicationDbContext context)
        {
            _context = context;
        }
        //public IActionResult Index()
        //{
        //    var get = _context.DoctorScheduleTbls.ToList();
        //    return View("../Doctor/DoctorSchedule/Schedule",get);
        //}



        [HttpGet]
        public async Task<IActionResult> AppointmentOperation()
        {
            var clinicDays = await _context.AppointmentScheduleTbls.ToListAsync();
            return View("../Doctor/DoctorSchedule/Schedule", clinicDays);
        }

        [HttpPost]
        public async Task<IActionResult> SaveClinicDays([FromBody] List<string> days)
        {
            if (days == null || days.Count == 0)
            {
                return BadRequest("Invalid days");
            }

            Console.WriteLine("Received days:", days);

            // Clear existing data
            _context.AppointmentScheduleTbls.RemoveRange(_context.AppointmentScheduleTbls);
            await _context.SaveChangesAsync();

            // Add new data
            foreach (var day in days)
            {
                _context.AppointmentScheduleTbls.Add(new AppointmentScheduleTbl { Date = day });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetClinicDays()
        {
            var clinicDays = await _context.AppointmentScheduleTbls.ToListAsync();
            return Json(clinicDays.Select(d => d.Date));
        }



        // GET: /AppointmentSchedule/GetDraggableEvents
        [HttpGet]
        public IActionResult GetDraggableEvents()
        {
            var events = _context.AppointmentScheduleTbls
                .Select(e => new
                {
                    e.ID,
                    e.Title,
                    e.DateOfEvents, // Assuming StartDate is a DateTime
                })
                .ToList();
            return Json(events);
        }

        // POST: /AppointmentSchedule/SaveDraggableEvent
        [HttpPost]
        public IActionResult SaveDraggableEvent([FromBody] AppointmentScheduleTbl draggableEvent)
        {
            if (ModelState.IsValid)
            {
                _context.AppointmentScheduleTbls.Add(draggableEvent);
                _context.SaveChanges();
                return Json(draggableEvent);
            }
            return BadRequest(ModelState);
        }

        // POST: /AppointmentSchedule/RemoveDraggableEvent
        [HttpPost]
        public IActionResult RemoveDraggableEvent([FromBody] long id)
        {
            var draggableEvent = _context.AppointmentScheduleTbls.Find(id);
            if (draggableEvent != null)
            {
                _context.AppointmentScheduleTbls.Remove(draggableEvent);
                _context.SaveChanges();
                return Json(draggableEvent);
            }
            return NotFound();
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
        public IActionResult SaveEvents([FromBody] EventsTbl eventModel)
        {
            if (ModelState.IsValid)
            {
                // Check if an event with the same date already exists
                var existingEvent = _context.EventsTbls.FirstOrDefault(e => e.Dates == eventModel.Dates);

                if (existingEvent != null)
                {
                    // If an event exists, remove it
                    _context.EventsTbls.Remove(existingEvent);
                }

                // Add the new event
                _context.EventsTbls.Add(new EventsTbl
                {
                    Dates = eventModel.Dates,
                    TItle = eventModel.TItle
                });

                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult DeleteEvent([FromBody] EventsTbl eventModel)
        {
            // Convert the string date from the model to DateTime
            DateTime eventDate;
            if (!DateTime.TryParse(eventModel.Dates, out eventDate))
            {
                return Json(new { success = false, message = "Invalid date format." });
            }

            // Convert the DateTime object to a string in the same format as your Dates field
            string eventDateString = eventDate.ToString("yyyy-MM-dd");

            // Use the string date directly in the query
            var eventToDelete = _context.EventsTbls
                                        .FirstOrDefault(e => e.TItle == eventModel.TItle && e.Dates == eventDateString);

            if (eventToDelete == null)
            {
                return Json(new { success = false, message = "Event not found." });
            }

            _context.EventsTbls.Remove(eventToDelete);
            _context.SaveChanges();

            return Json(new { success = true });
        }

    }
}

using Microsoft.EntityFrameworkCore;
using VetClinicCapstone.Models;

namespace VetClinicCapstone.EmailNotication
{
    public class AppointmentNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public AppointmentNotificationService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task SendDailyAppointmentNotifications()
        {
            var tomorrow = DateTime.Today.AddDays(1);
			//var appointments = await _context.AppointmentTbls
			//    .Where(a => a.Date.Date == tomorrow && a.Status == "Approved")
			//    .Include(a => a.Owner)
			//    .ToListAsync();
			var appointments = await _context.AppointmentTbls
	.Where(a => a.Date.Date == tomorrow && a.Status == "Approved" && !string.IsNullOrEmpty(a.Owner.Email))
	.Include(a => a.Owner)
	.ToListAsync();
			foreach (var appointment in appointments)
            {
                if (appointment.Owner != null)
                {
                    var ownerEmail = appointment.Owner.Email;
                    var time = appointment.Time;
                    var Date = appointment.Date.ToString("MMMM dd, yyyy");
                    var ownerName = appointment.Owner.Firstname;
                    var subject = "Appointment Reminder";
                    var message = $@"Dear {ownerName},

This is a friendly reminder for your upcoming appointment at Urvan Vet Animal Clinic.

Appointment Details:
- Date: {Date}
- Time: {time}
- Location: Dio Commercial Complex, Mabayabas, Taysan, Batangas

Thank you for choosing Urvan Vet Animal Clinic. We look forward to seeing you and your pet.

Best regards,
Urvan Vet Animal Clinic
		      ";

                    await _emailService.SendEmailAsync(ownerEmail, subject, message);
                }
            }
        }
    }
}

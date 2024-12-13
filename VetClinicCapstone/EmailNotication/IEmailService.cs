namespace VetClinicCapstone.EmailNotication
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string message);
	}
}

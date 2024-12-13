using VetClinicCapstone.EmailNotication;

namespace VetClinicCapstone.BackgroundServices
{
    public class AppointmentNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AppointmentNotificationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<AppointmentNotificationService>();
                    await notificationService.SendDailyAppointmentNotifications();
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}

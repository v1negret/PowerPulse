using Microsoft.EntityFrameworkCore;
using PowerPulse.Infrastructure.Data;

namespace PowerPulse.Modules.Email.Services;

public class ReminderService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public ReminderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EnergyDbContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<EmailSenderService>();

                var now = DateTime.UtcNow;
                var oneMonthAgo = now.AddMonths(-1);

                var usersToRemind = await dbContext.Users
                    .Where(u => u.LastReminderSentDate <= oneMonthAgo &&
                                !u.MeterReadings.Any(mr => mr.CreateDate >= oneMonthAgo))
                    .ToListAsync();

                foreach (var user in usersToRemind)
                {
                    var subject = "Напоминание: Пожалуйста, введите показания электроэнергии";
                    var message = "Уважаемый пользователь, прошло более месяца с момента вашего последнего ввода показаний электроэнергии. Пожалуйста, обновите ваши данные.";
                    await emailSender.SendEmailAsync(user.Email, subject, message);
                    user.LastReminderSentDate = now;
                }

                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в ReminderService: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
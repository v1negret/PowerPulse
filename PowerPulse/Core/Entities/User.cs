namespace PowerPulse.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime LastReminderSentDate { get; set; }
    public List<MeterReading> MeterReadings { get; set; } = new();
}
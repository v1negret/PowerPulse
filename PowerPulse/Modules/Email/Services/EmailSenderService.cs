using System.Net;
using MailKit.Net.Smtp;
using MimeKit;

namespace PowerPulse.Modules.Email.Services;

public class EmailSenderService
{
    private readonly IConfiguration _configuration;
    public EmailSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        var fromAddress = new MailboxAddress("PowerPulse", _configuration["EmailSettings:SmtpUsername"]);
        var toAddress = new MailboxAddress("", email);
        emailMessage.From.Add(fromAddress);
        emailMessage.To.Add(toAddress);
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = message };

        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]), true);
        await client.AuthenticateAsync("powerpulsemail", _configuration["EmailSettings:SmtpPassword"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
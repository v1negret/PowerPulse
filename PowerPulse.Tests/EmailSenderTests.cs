using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PowerPulse.Modules.Email.Services;

namespace PowerPulse.Tests;


public class EmailSenderTests
{
    private IConfiguration _configuration;

    private readonly Dictionary<string, string> _confDictionary = new()
    {
        {"EmailSettings:SmtpServer", "smtp.yandex.com"},
        {"EmailSettings:SmtpPort", "465"},
        {"EmailSettings:SenderName", "PowerPulse"},
        {"EmailSettings:SmtpUsername", "powerpulsemail@yandex.com"},
        {"EmailSettings:SmtpPassword", "ejfscznxnmcnkxuc"},
    };

    public EmailSenderTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_confDictionary!)
            .Build();
    }
    
    [Fact]
    public async Task EmailSendTest()
    {
        // Arrange
        var emailSenderService = new EmailSenderService(_configuration);
        
        //Act
        var action = async () => await emailSenderService.SendEmailAsync("test@mail.ru", "Тест", "Тестровое сообщение");
        
        //Assert
        await action.Should().NotThrowAsync();
    }
}
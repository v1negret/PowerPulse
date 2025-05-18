using Microsoft.EntityFrameworkCore;
using PowerPulse.Core.Entities;
using PowerPulse.Infrastructure.Data;
using PowerPulse.Modules.EnergyConsumption.Services;

namespace PowerPulse.Tests
{
    public class ConsumptionServiceTests
    {
        [Fact]
        public async Task AddMeterReading_ShouldAddReadingToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EnergyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new EnergyDbContext(options);
            var user = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", PasswordHash = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var consumptionService = new ConsumptionService(context);

            // Act
            var date = DateTime.UtcNow;
            await consumptionService.AddMeterReading(user.Id, date, 100.0, 50.0m);

            // Assert
            var reading = await context.MeterReadings.FirstOrDefaultAsync(r => r.UserId == user.Id && r.Date == date);
            Assert.NotNull(reading);
            Assert.Equal(100.0, reading.Reading);
            Assert.Equal(50.0m, reading.Cost);
        }
    }
}
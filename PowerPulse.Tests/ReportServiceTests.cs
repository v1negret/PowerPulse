using Microsoft.EntityFrameworkCore;
using PowerPulse.Core.Entities;
using PowerPulse.Infrastructure.Data;
using PowerPulse.Modules.Report.Services;


namespace PowerPulse.Tests
{
    public class ReportServiceTests
    {
        [Fact]
        public async Task GetKeyMetricsAsync_ShouldReturnCorrectMetrics()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EnergyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new EnergyDbContext(options);
            var userId = Guid.NewGuid();
            context.MeterReadings.AddRange(
                new MeterReading { Id = Guid.NewGuid(), UserId = userId, Date = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc), Reading = 100, Cost = 50 },
                new MeterReading { Id = Guid.NewGuid(), UserId = userId, Date = new DateTime(2023, 2, 1, 0, 0, 0, DateTimeKind.Utc), Reading = 200, Cost = 100 }
            );
            
            
            await context.SaveChangesAsync();

            var reportService = new ReportService(context);
            var from = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var metrics = await reportService.GetKeyMetricsAsync(userId, from, to);

            // Assert
            Assert.Equal(300, metrics.TotalConsumption);
            Assert.Equal(150, metrics.TotalCost);
            Assert.Equal(150, metrics.AvgConsumption);
            Assert.Equal(0.5m, metrics.AvgCostPerKwh);
        }
    }
}
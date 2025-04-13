using Microsoft.EntityFrameworkCore;
using PowerPulse.Core.Entities;
using PowerPulse.Infrastructure.Data;

namespace PowerPulse.Modules.EnergyConsumption.Services;

public class ConsumptionService
{
    private readonly EnergyDbContext _context;

    public ConsumptionService(EnergyDbContext context)
    {
        _context = context;
    }

    public async Task AddMeterReading(Guid userId, DateTime date, double reading, decimal cost)
    {
        var meterReading = new MeterReading
        {
            UserId = userId,
            Date = date,
            Reading = reading,
            Cost = cost,
        };
        _context.MeterReadings.Add(meterReading);
        await _context.SaveChangesAsync();
    }

    public async Task<(double Min, double Max, DateTime MinMonth, DateTime MaxMonth)> GetMinMaxConsumption(Guid userId, int year)
    {
        var readings = await _context.MeterReadings
            .Where(m => m.UserId == userId && m.Date.Year == year)
            .ToListAsync();

        if (!readings.Any()) return (0, 0, DateTime.MinValue, DateTime.MinValue);

        var minReading = readings.OrderBy(m => m.Reading).First();
        var maxReading = readings.OrderByDescending(m => m.Reading).First();

        return (minReading.Reading, maxReading.Reading, minReading.Date, maxReading.Date);
    }
}
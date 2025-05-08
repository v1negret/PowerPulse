using Microsoft.EntityFrameworkCore;
using PowerPulse.Core.Entities;
using PowerPulse.Infrastructure.Data;
using PowerPulse.Modules.EnergyConsumption.Models;

namespace PowerPulse.Modules.EnergyConsumption.Services;

public class ConsumptionService
{
    private readonly EnergyDbContext _context;

    public ConsumptionService(EnergyDbContext context)
    {
        _context = context;
    }

    public async Task AddMeterReading(Guid userId, DateTime date, double reading, decimal? cost)
    {
        var meterReading = new MeterReading
        {
            Id = Guid.NewGuid(),
            Date = date,
            Reading = reading,
            Cost = cost,
            UserId = userId,
            CreateDate = DateTime.UtcNow
        };
        _context.MeterReadings.Add(meterReading);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> Update(MeterReadingModel model, Guid userId)
    {
        var first = await _context.MeterReadings.FirstOrDefaultAsync(x => x.Id == model.Id && x.UserId == userId);
        if (first == null)
            return false;
        
        first.Date = model.Date;
        first.Cost = model.Cost;
        first.Reading = model.Reading;
        
        _context.MeterReadings.Update(first);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<MeterReading>> GetByUserId(Guid userUid)
        => await _context.MeterReadings
            .Where(x => x.UserId == userUid)
            .ToListAsync();

    public async Task<bool> DeleteById(Guid readingsUid, Guid userUid)
    {
        var first = await _context.MeterReadings.FirstOrDefaultAsync(x => x.UserId == userUid && x.Id == readingsUid);
        if(first is null)
            return false;
        _context.MeterReadings.Remove(first);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MeterReading?> GetByDateAndUserId(Guid userId, DateTime date)
    {
        var result = await _context.MeterReadings.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date);
        
        return result;
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
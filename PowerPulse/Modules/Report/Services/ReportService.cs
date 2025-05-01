using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using PowerPulse.Infrastructure.Data;
using PowerPulse.Modules.Report.Models;

namespace PowerPulse.Modules.Report.Services;

public class ReportService
{
    private readonly EnergyDbContext _context;

    public ReportService(EnergyDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateReportAsync(int year, Guid userId)
    {
        var from = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var keyMetrics = await GetKeyMetricsAsync(userId, from, to);
        var costPerKwh = await GetCostPerKwhAsync(userId, from, to);
        var totalCost = await GetTotalCostAsync(userId, from, to);
        var avgConsumption = await GetAverageConsumptionAsync(userId, from, to);

        if (!costPerKwh.Any() && !totalCost.Any() && !avgConsumption.Any())
            throw new InvalidOperationException("Нет данных для выбранного года.");

        var excelService = new ExcelService();
        return excelService.GenerateExcelReport(year, keyMetrics, costPerKwh, totalCost, avgConsumption);
    }

    private async Task<KeyMetricsDto> GetKeyMetricsAsync(Guid userId, DateTime from, DateTime to)
    {
        var metrics = await _context.MeterReadings
            .Where(r => r.UserId == userId && r.Date >= from && r.Date <= to)
            .GroupBy(mr => 1)
            .Select(g => new KeyMetricsDto
            {
                TotalConsumption = g.Sum(r => r.Reading),
                TotalCost = g.Sum(r => r.Cost),
                AvgConsumption = g.Average(r => r.Reading),
                AvgCostPerKwh = g.Average(r => r.Cost / (decimal)r.Reading)
            })
            .FirstOrDefaultAsync();

        return metrics ?? new KeyMetricsDto();
    }

    private async Task<List<TimeSeriesDto>> GetCostPerKwhAsync(Guid userId, DateTime from, DateTime to)
    {
        return await _context.MeterReadings
            .Where(r => r.UserId == userId && r.Date >= from && r.Date <= to)
            .GroupBy(r => new { Date = r.Date.Date })
            .Select(g => new TimeSeriesDto
            {
                Date = g.Key.Date,
                Value = g.Average(r => (decimal)r.Cost / (decimal)r.Reading)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    private async Task<List<TimeSeriesDto>> GetTotalCostAsync(Guid userId, DateTime from, DateTime to)
    {
        return await _context.MeterReadings
            .Where(r => r.UserId == userId && r.Date >= from && r.Date <= to)
            .GroupBy(r => new { Date = r.Date.Date })
            .Select(g => new TimeSeriesDto
            {
                Date = g.Key.Date,
                Value = g.Sum(r => r.Cost)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }

    private async Task<List<TimeSeriesDto>> GetAverageConsumptionAsync(Guid userId, DateTime from, DateTime to)
    {
        return await _context.MeterReadings
            .Where(r => r.UserId == userId && r.Date >= from && r.Date <= to)
            .GroupBy(r => new { Date = r.Date.Date })
            .Select(g => new TimeSeriesDto
            {
                Date = g.Key.Date,
                Value = g.Average(r => (decimal)r.Reading)
            })
            .OrderBy(r => r.Date)
            .ToListAsync();
    }
}
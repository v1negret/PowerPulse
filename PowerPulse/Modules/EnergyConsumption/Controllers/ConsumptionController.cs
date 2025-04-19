using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPulse.Modules.EnergyConsumption.Models;
using PowerPulse.Modules.EnergyConsumption.Services;

namespace PowerPulse.Modules.EnergyConsumption.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ConsumptionController : ControllerBase
{
    private readonly ConsumptionService _consumptionService;

    public ConsumptionController(ConsumptionService consumptionService)
    {
        _consumptionService = consumptionService;
    }

    [HttpPost("meter-reading")]
    public async Task<IActionResult> AddMeterReading([FromBody]MeterReadingModel model)
    {
        model.Date = model.Date.ToUniversalTime();
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _consumptionService.AddMeterReading(userId, model.Date, model.Reading, model.Cost);
        return Ok();
    }

    [HttpGet("min-max/{year}")]
    public async Task<IActionResult> GetMinMaxConsumption(int year)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _consumptionService.GetMinMaxConsumption(userId, year);
        return Ok(new
        {
            MinConsumption = result.Min,
            MaxConsumption = result.Max,
            MinMonth = result.MinMonth,
            MaxMonth = result.MaxMonth
        });
    }
}
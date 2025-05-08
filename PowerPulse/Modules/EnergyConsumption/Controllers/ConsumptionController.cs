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

    [HttpPost("meter-readings")]
    public async Task<IActionResult> AddMeterReading([FromBody]NullableMeterReadingModel model)
    {
        model.Date = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, 0, 0, 0, DateTimeKind.Local);
        model.Date = model.Date.ToUniversalTime();
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var existingReading = await _consumptionService.GetByDateAndUserId(userId, model.Date);
        if (existingReading is not null)
            return BadRequest("На эту дату уже добавлены показания");
        await _consumptionService.AddMeterReading(userId, model.Date, model.Reading, model.Cost);
        return Ok();
    }
    
    [HttpPut("meter-readings")]
    public async Task<IActionResult> UpdateReading([FromBody]MeterReadingModel model)
    {
        model.Date = model.Date.ToUniversalTime();
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (await _consumptionService.Update(model, userId))
            return Ok();
        return BadRequest("Не удалось обновить показания. Попробуйте ещё раз позже");
    }

    [HttpGet("meter-readings")]
    public async Task<IActionResult> GetByUser()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var result = await _consumptionService.GetByUserId(userId);
        return Ok(result);
    }
    
    [HttpDelete("meter-readings/remove/{readingsId:guid}")]
    public async Task<IActionResult> Delete(Guid readingsId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (await _consumptionService.DeleteById(readingsId, userId))
            return Ok();
        return BadRequest("Не удалось удалить показания. Попробуйте ещё раз позже");
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
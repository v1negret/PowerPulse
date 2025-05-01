using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPulse.Modules.Report.Services;

namespace PowerPulse.Modules.Report.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("generate/{year}")]
        public async Task<IActionResult> GenerateReport(int year)
        {
            try
            {
                var userIdClaim = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (userIdClaim == Guid.Empty)
                {
                    return Unauthorized("Пользователь не авторизован.");
                }

                var reportBytes = await _reportService.GenerateReportAsync(year, userIdClaim);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EnergyReport_{year}.xlsx");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при генерации отчёта: {ex.Message}");
            }
        }
    }
}
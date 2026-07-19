using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("risk-assessment")]
    public async Task<IActionResult> GetRiskAssessmentReport()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRiskAssessmentReport(userId);
        if (report == null)
            return NotFound(new { message = "No risk assessment found. Please complete the questionnaire first." });

        return Ok(report);
    }

    [HttpGet("recommendation")]
    public async Task<IActionResult> GetRecommendationReport()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRecommendationReport(userId);
        if (report == null)
            return NotFound(new { message = "No recommendation found. Please generate a recommendation first." });

        return Ok(report);
    }

    [HttpGet("portfolio")]
    public async Task<IActionResult> GetPortfolioReport()
    {
        var userId = GetUserId();
        var report = await _reportService.GeneratePortfolioReport(userId);
        if (report == null)
            return NotFound(new { message = "No portfolio found." });

        return Ok(report);
    }

    [HttpPost("stress-test")]
    public async Task<IActionResult> RunStressTest([FromBody] StressTestRequestDTO? request)
    {
        var userId = GetUserId();

        // If no custom scenarios provided, use defaults
        request ??= new StressTestRequestDTO
        {
            Scenarios = new List<StressScenarioDTO>
            {
                new() { Name = "10% Market Decline", PercentageChange = -10 },
                new() { Name = "20% Market Decline", PercentageChange = -20 },
                new() { Name = "30% Market Decline", PercentageChange = -30 },
                new() { Name = "Bull Market (+20%)", PercentageChange = 20 },
                new() { Name = "Financial Crisis (-50%)", PercentageChange = -50 }
            }
        };

        var report = await _reportService.GenerateStressTestReport(userId, request);
        return Ok(report);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

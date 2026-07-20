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
    private readonly PdfReportService _pdfService;

    public ReportController(ReportService reportService, PdfReportService pdfService)
    {
        _reportService = reportService;
        _pdfService = pdfService;
    }

    [HttpGet("risk-assessment")]
    public async Task<IActionResult> GetRiskAssessmentReport()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRiskAssessmentReport(userId);
        if (report == null)
            return NotFound(new { message = "No risk assessment found." });
        return Ok(report);
    }

    [HttpGet("risk-assessment/pdf")]
    public async Task<IActionResult> GetRiskAssessmentPdf()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRiskAssessmentReport(userId);
        if (report == null)
            return NotFound(new { message = "No risk assessment found." });
        var html = _pdfService.GenerateRiskAssessmentHtml(report);
        return Content(html, "text/html");
    }

    [HttpGet("recommendation")]
    public async Task<IActionResult> GetRecommendationReport()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRecommendationReport(userId);
        if (report == null)
            return NotFound(new { message = "No recommendation found." });
        return Ok(report);
    }

    [HttpGet("recommendation/pdf")]
    public async Task<IActionResult> GetRecommendationPdf()
    {
        var userId = GetUserId();
        var report = await _reportService.GenerateRecommendationReport(userId);
        if (report == null)
            return NotFound(new { message = "No recommendation found." });
        var html = _pdfService.GenerateRecommendationHtml(report);
        return Content(html, "text/html");
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

    [HttpGet("portfolio/pdf")]
    public async Task<IActionResult> GetPortfolioPdf()
    {
        var userId = GetUserId();
        var report = await _reportService.GeneratePortfolioReport(userId);
        if (report == null)
            return NotFound(new { message = "No portfolio found." });
        var html = _pdfService.GeneratePortfolioHtml(report);
        return Content(html, "text/html");
    }

    [HttpPost("stress-test")]
    public async Task<IActionResult> RunStressTest([FromBody] StressTestRequestDTO? request)
    {
        var userId = GetUserId();
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

    [HttpPost("stress-test/pdf")]
    public async Task<IActionResult> RunStressTestPdf([FromBody] StressTestRequestDTO? request)
    {
        var userId = GetUserId();
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
        var html = _pdfService.GenerateStressTestHtml(report);
        return Content(html, "text/html");
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/portfolio")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly PortfolioService _portfolioService;

    public PortfolioController(PortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPortfolio()
    {
        var userId = GetUserId();
        var portfolio = await _portfolioService.GetPortfolio(userId);
        if (portfolio == null)
            return NotFound(new { message = "No portfolio found. Start by adding your holdings." });

        return Ok(portfolio);
    }

    [HttpPost("holdings")]
    public async Task<IActionResult> AddHolding([FromBody] AddHoldingDTO dto)
    {
        var userId = GetUserId();
        var result = await _portfolioService.AddHolding(userId, dto);
        return Ok(result);
    }

    [HttpDelete("holdings/{holdingId}")]
    public async Task<IActionResult> RemoveHolding(int holdingId)
    {
        var userId = GetUserId();
        var removed = await _portfolioService.RemoveHolding(userId, holdingId);
        if (!removed)
            return NotFound(new { message = "Holding not found" });

        return Ok(new { message = "Holding removed successfully" });
    }

    [HttpGet("analyze")]
    public async Task<IActionResult> AnalyzePortfolio()
    {
        var userId = GetUserId();
        var analysis = await _portfolioService.AnalyzePortfolio(userId);
        if (analysis == null)
            return NotFound(new { message = "No portfolio found or portfolio is empty." });

        return Ok(analysis);
    }

    [HttpPost("upload/csv")]
    public async Task<IActionResult> UploadCsv(IFormFile file, [FromServices] FileUploadService fileService)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Please upload a .csv file" });

        var userId = GetUserId();
        using var stream = file.OpenReadStream();
        var holdings = await fileService.ParseCsvFile(stream);

        if (!holdings.Any())
            return BadRequest(new { message = "No valid holdings found in file" });

        var added = new List<HoldingDTO>();
        foreach (var h in holdings)
        {
            var result = await _portfolioService.AddHolding(userId, h);
            added.Add(result);
        }

        return Ok(new { message = $"{added.Count} holdings imported successfully", holdings = added });
    }

    [HttpPost("upload/excel")]
    public async Task<IActionResult> UploadExcel(IFormFile file, [FromServices] FileUploadService fileService)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Please upload a .xlsx file" });

        var userId = GetUserId();
        using var stream = file.OpenReadStream();
        var holdings = await fileService.ParseExcelFile(stream);

        if (!holdings.Any())
            return BadRequest(new { message = "No valid holdings found in file" });

        var added = new List<HoldingDTO>();
        foreach (var h in holdings)
        {
            var result = await _portfolioService.AddHolding(userId, h);
            added.Add(result);
        }

        return Ok(new { message = $"{added.Count} holdings imported successfully", holdings = added });
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

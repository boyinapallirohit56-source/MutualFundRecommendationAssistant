using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/funds")]
[Authorize]
public class FundController : ControllerBase
{
    private readonly FundService _fundService;

    public FundController(FundService fundService)
    {
        _fundService = fundService;
    }

    [HttpGet]
    public async Task<IActionResult> ListFunds([FromQuery] string? category = null, [FromQuery] string? search = null)
    {
        var funds = await _fundService.ListFunds(category, search);
        return Ok(funds);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFundFactsheet(int id)
    {
        var factsheet = await _fundService.GetFundFactsheet(id);
        if (factsheet == null)
            return NotFound(new { message = "Fund not found" });

        return Ok(factsheet);
    }

    [HttpPost("compare")]
    public async Task<IActionResult> CompareFunds([FromBody] FundComparisonRequestDTO dto)
    {
        try
        {
            var comparison = await _fundService.CompareFunds(dto.FundIds);
            return Ok(comparison);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

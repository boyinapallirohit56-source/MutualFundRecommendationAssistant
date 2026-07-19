using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/recommendations")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _recommendationService;

    public RecommendationController(RecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateRecommendation()
    {
        var userId = GetUserId();
        var result = await _recommendationService.GenerateRecommendation(userId);
        if (result == null)
            return BadRequest(new { message = "Please complete the risk assessment first." });

        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRecommendation()
    {
        var userId = GetUserId();
        var result = await _recommendationService.GetLatestRecommendation(userId);
        if (result == null)
            return NotFound(new { message = "No recommendation found. Please generate one." });

        return Ok(result);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

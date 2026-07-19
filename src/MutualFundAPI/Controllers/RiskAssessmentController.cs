using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/risk-assessment")]
[Authorize]
public class RiskAssessmentController : ControllerBase
{
    private readonly RiskAssessmentService _assessmentService;

    public RiskAssessmentController(RiskAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestions()
    {
        var questions = await _assessmentService.GetQuestions();
        return Ok(questions);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAssessment([FromBody] SubmitAssessmentDTO dto)
    {
        var userId = GetUserId();
        var result = await _assessmentService.SubmitAssessment(userId, dto);
        if (result == null)
            return BadRequest(new { message = "Failed to process assessment" });

        return Ok(result);
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestAssessment()
    {
        var userId = GetUserId();
        var result = await _assessmentService.GetLatestAssessment(userId);
        if (result == null)
            return NotFound(new { message = "No assessment found. Please complete the questionnaire." });

        return Ok(result);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

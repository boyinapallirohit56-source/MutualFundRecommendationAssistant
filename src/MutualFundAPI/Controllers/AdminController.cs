using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    // --- Users ---

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsers();
        return Ok(users);
    }

    [HttpPut("users/{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(int userId, [FromBody] UpdateUserStatusDTO dto)
    {
        var result = await _adminService.UpdateUserStatus(userId, dto.IsActive);
        if (!result)
            return NotFound(new { message = "User not found" });

        return Ok(new { message = "User status updated" });
    }

    // --- Questions ---

    [HttpPost("questions")]
    public async Task<IActionResult> AddQuestion([FromBody] AdminQuestionDTO dto)
    {
        var question = await _adminService.AddQuestion(dto);
        return Ok(question);
    }

    [HttpPut("questions/{questionId}")]
    public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] AdminQuestionDTO dto)
    {
        var result = await _adminService.UpdateQuestion(questionId, dto);
        if (!result)
            return NotFound(new { message = "Question not found" });

        return Ok(new { message = "Question updated" });
    }

    [HttpDelete("questions/{questionId}")]
    public async Task<IActionResult> DeleteQuestion(int questionId)
    {
        var result = await _adminService.DeleteQuestion(questionId);
        if (!result)
            return NotFound(new { message = "Question not found" });

        return Ok(new { message = "Question deactivated" });
    }

    // --- Funds ---

    [HttpPost("funds")]
    public async Task<IActionResult> AddFund([FromBody] AdminFundDTO dto)
    {
        var fund = await _adminService.AddFund(dto);
        return Ok(fund);
    }

    [HttpPut("funds/{fundId}")]
    public async Task<IActionResult> UpdateFund(int fundId, [FromBody] AdminFundDTO dto)
    {
        var result = await _adminService.UpdateFund(fundId, dto);
        if (!result)
            return NotFound(new { message = "Fund not found" });

        return Ok(new { message = "Fund updated" });
    }

    [HttpDelete("funds/{fundId}")]
    public async Task<IActionResult> DeleteFund(int fundId)
    {
        var result = await _adminService.DeleteFund(fundId);
        if (!result)
            return NotFound(new { message = "Fund not found" });

        return Ok(new { message = "Fund deactivated" });
    }

    // --- Analytics ---

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var analytics = await _adminService.GetAnalytics();
        return Ok(analytics);
    }

    // --- AMFI Data Sync ---

    [HttpPost("sync-amfi")]
    public async Task<IActionResult> SyncAmfiData([FromServices] AmfiDataService amfiService)
    {
        var result = await amfiService.SyncNavData();
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = $"Sync complete. Processed: {result.Processed}, Updated: {result.Updated}" });
    }

    [HttpPost("import-amfi/{category}")]
    public async Task<IActionResult> ImportAmfiByCategory(string category, [FromServices] AmfiDataService amfiService, [FromQuery] int maxFunds = 20)
    {
        var result = await amfiService.ImportFundsByCategory(category, maxFunds);
        if (!result.Success)
            return BadRequest(new { message = result.ErrorMessage });

        return Ok(new { message = $"Imported {result.Updated} funds for category: {category}" });
    }

    // --- Allocation Rules ---

    [HttpGet("allocation-rules")]
    public async Task<IActionResult> GetAllocationRules()
    {
        var rules = await _adminService.GetAllocationRules();
        return Ok(rules);
    }

    [HttpPut("allocation-rules/{riskProfile}")]
    public async Task<IActionResult> UpdateAllocationRules(string riskProfile, [FromBody] UpdateAllocationRulesDTO dto)
    {
        var result = await _adminService.UpdateAllocationRules(riskProfile, dto.Allocations);
        if (!result)
            return NotFound(new { message = "No rules found for this profile" });

        return Ok(new { message = $"Allocation rules updated for {riskProfile}" });
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Single aggregation endpoint returning all dashboard data
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = GetUserId();
        var dashboard = await _dashboardService.GetDashboard(userId);
        return Ok(dashboard);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

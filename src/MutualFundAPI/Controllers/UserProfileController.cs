using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;

    public UserProfileController(UserProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        var profile = await _profileService.GetProfile(userId);
        if (profile == null)
            return NotFound(new { message = "Profile not found. Please complete onboarding." });

        return Ok(profile);
    }

    [HttpPost("profile")]
    public async Task<IActionResult> SaveProfile([FromBody] UserProfileDTO dto)
    {
        var userId = GetUserId();
        var result = await _profileService.SaveProfile(userId, dto);
        return Ok(result);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

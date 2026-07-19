using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/watchlist")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly WatchlistService _watchlistService;

    public WatchlistController(WatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWatchlist()
    {
        var userId = GetUserId();
        var items = await _watchlistService.GetWatchlist(userId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWatchlist([FromBody] AddToWatchlistDTO dto)
    {
        var userId = GetUserId();
        var result = await _watchlistService.AddToWatchlist(userId, dto.MutualFundId);
        if (result == null)
            return BadRequest(new { message = "Fund already in watchlist or not found" });

        return Ok(result);
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveFromWatchlist(int itemId)
    {
        var userId = GetUserId();
        var removed = await _watchlistService.RemoveFromWatchlist(userId, itemId);
        if (!removed)
            return NotFound(new { message = "Item not found in watchlist" });

        return Ok(new { message = "Removed from watchlist" });
    }

    [HttpGet("check/{fundId}")]
    public async Task<IActionResult> IsInWatchlist(int fundId)
    {
        var userId = GetUserId();
        var exists = await _watchlistService.IsInWatchlist(userId, fundId);
        return Ok(new { isInWatchlist = exists });
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

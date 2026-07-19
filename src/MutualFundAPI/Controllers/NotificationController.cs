using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int count = 20)
    {
        var userId = GetUserId();
        var notifications = await _notificationService.GetNotifications(userId, count);
        return Ok(notifications);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var userId = GetUserId();
        var count = await _notificationService.GetNotificationCount(userId);
        return Ok(count);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var userId = GetUserId();
        await _notificationService.MarkAsRead(userId, id);
        return Ok(new { message = "Marked as read" });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetUserId();
        await _notificationService.MarkAllAsRead(userId);
        return Ok(new { message = "All marked as read" });
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

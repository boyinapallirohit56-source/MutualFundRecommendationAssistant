using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Services;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public class AIChatController : ControllerBase
{
    private readonly AIChatService _chatService;

    public AIChatController(AIChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { message = "Message cannot be empty" });

        var userId = GetUserId();
        var result = await _chatService.SendMessage(userId, dto.Message, dto.CurrentPage);
        return Ok(result);
    }

    [HttpGet("chat/history")]
    public async Task<IActionResult> GetChatHistory([FromQuery] int count = 20)
    {
        var userId = GetUserId();
        var history = await _chatService.GetChatHistory(userId, count);
        return Ok(history);
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

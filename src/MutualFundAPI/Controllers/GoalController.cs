using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Controllers;

[ApiController]
[Route("api/v1/goals")]
[Authorize]
public class GoalController : ControllerBase
{
    private readonly AppDbContext _context;

    public GoalController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetGoals()
    {
        var userId = GetUserId();
        var goals = await _context.Goals
            .Where(g => g.UserId == userId && g.IsActive)
            .Select(g => new GoalResponseDTO
            {
                Id = g.Id,
                Name = g.Name,
                TargetAmount = g.TargetAmount,
                CurrentAmount = g.CurrentAmount,
                TargetYears = g.TargetYears,
                MonthlySIP = g.MonthlySIP,
                ProgressPercentage = g.TargetAmount > 0 ? Math.Round((g.CurrentAmount / g.TargetAmount) * 100, 1) : 0,
                IsActive = g.IsActive
            })
            .ToListAsync();

        return Ok(goals);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalDTO dto)
    {
        var userId = GetUserId();

        // Check if goal with same name already exists for this user
        var existing = await _context.Goals
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Name == dto.Name && g.IsActive);

        if (existing != null)
        {
            // Update existing goal
            existing.TargetAmount = dto.TargetAmount;
            existing.TargetYears = dto.TargetYears;
            existing.MonthlySIP = dto.MonthlySIP;
        }
        else
        {
            // Create new goal
            var goal = new Goal
            {
                UserId = userId,
                Name = dto.Name,
                TargetAmount = dto.TargetAmount,
                CurrentAmount = 0,
                TargetYears = dto.TargetYears,
                MonthlySIP = dto.MonthlySIP
            };
            _context.Goals.Add(goal);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Goal saved successfully" });
    }

    [HttpPost("batch")]
    public async Task<IActionResult> CreateGoalsBatch([FromBody] List<CreateGoalDTO> goals)
    {
        var userId = GetUserId();

        foreach (var dto in goals)
        {
            var existing = await _context.Goals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.Name == dto.Name && g.IsActive);

            if (existing != null)
            {
                existing.TargetAmount = dto.TargetAmount;
                existing.TargetYears = dto.TargetYears;
                existing.MonthlySIP = dto.MonthlySIP;
            }
            else
            {
                _context.Goals.Add(new Goal
                {
                    UserId = userId,
                    Name = dto.Name,
                    TargetAmount = dto.TargetAmount,
                    CurrentAmount = 0,
                    TargetYears = dto.TargetYears,
                    MonthlySIP = dto.MonthlySIP
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"{goals.Count} goals saved successfully" });
    }

    [HttpPut("{goalId}/progress")]
    public async Task<IActionResult> UpdateProgress(int goalId, [FromBody] UpdateGoalProgressDTO dto)
    {
        var userId = GetUserId();
        var goal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

        if (goal == null)
            return NotFound(new { message = "Goal not found" });

        goal.CurrentAmount = dto.CurrentAmount;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Progress updated", progressPercentage = goal.ProgressPercentage });
    }

    [HttpDelete("{goalId}")]
    public async Task<IActionResult> DeleteGoal(int goalId)
    {
        var userId = GetUserId();
        var goal = await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

        if (goal == null)
            return NotFound(new { message = "Goal not found" });

        goal.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Goal removed" });
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
}

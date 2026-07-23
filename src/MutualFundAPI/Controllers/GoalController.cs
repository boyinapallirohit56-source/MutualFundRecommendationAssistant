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

        // Get user's financial profile to calculate realistic initial progress
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        decimal userSavings = profile?.Savings ?? 0;
        decimal userSIPAmount = profile?.SIPAmount ?? 0;
        string existingInvestments = profile?.ExistingInvestments ?? "None";

        // Calculate how much of savings to allocate across goals
        decimal totalTargetAcrossGoals = goals.Sum(g => g.TargetAmount);
        int goalCount = goals.Count;

        foreach (var dto in goals)
        {
            var existing = await _context.Goals
                .FirstOrDefaultAsync(g => g.UserId == userId && g.Name == dto.Name && g.IsActive);

            // Calculate initial CurrentAmount based on user's financial profile
            decimal initialAmount = CalculateInitialProgress(
                dto.TargetAmount, dto.TargetYears,
                userSavings, userSIPAmount, existingInvestments, goalCount);

            if (existing != null)
            {
                existing.TargetAmount = dto.TargetAmount;
                existing.TargetYears = dto.TargetYears;
                existing.MonthlySIP = dto.MonthlySIP;
                existing.CurrentAmount = initialAmount;
            }
            else
            {
                _context.Goals.Add(new Goal
                {
                    UserId = userId,
                    Name = dto.Name,
                    TargetAmount = dto.TargetAmount,
                    CurrentAmount = initialAmount,
                    TargetYears = dto.TargetYears,
                    MonthlySIP = dto.MonthlySIP
                });
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"{goals.Count} goals saved successfully" });
    }

    /// <summary>
    /// Calculates realistic initial progress based on user's financial profile.
    /// Not random — derived from actual savings, investment history, and SIP amount.
    /// </summary>
    private static decimal CalculateInitialProgress(
        decimal targetAmount, int targetYears,
        decimal userSavings, decimal userSIPAmount,
        string existingInvestments, int totalGoals)
    {
        if (targetAmount <= 0) return 0;

        decimal initialAmount = 0;

        // Normalize the investment type string for robust matching
        string investmentType = (existingInvestments ?? "").Trim().ToLower();

        // Factor 1: Allocate a portion of existing savings toward this goal
        // (Split savings across all goals proportionally)
        if (userSavings > 0 && totalGoals > 0)
        {
            decimal savingsPerGoal = userSavings / totalGoals;
            // Don't allocate more than 30% of target from savings alone
            initialAmount += Math.Min(savingsPerGoal, targetAmount * 0.30m);
        }

        // Factor 2: If user has existing investments, they likely have some progress
        decimal investmentMultiplier = 0m;
        if (investmentType.Contains("mutual"))
            investmentMultiplier = 0.10m;      // Already invests in MFs — assume 10% progress
        else if (investmentType.Contains("stock"))
            investmentMultiplier = 0.08m;      // Stocks investor — assume 8%
        else if (investmentType.Contains("multiple") || investmentType.Contains("diversi"))
            investmentMultiplier = 0.15m;      // Diversified — assume 15% progress
        else if (investmentType.Contains("fd") || investmentType.Contains("rd"))
            investmentMultiplier = 0.05m;      // Conservative — assume 5%

        initialAmount += targetAmount * investmentMultiplier;

        // Factor 3: If user has SIP running, assume a few months already invested
        if (userSIPAmount > 0)
        {
            // Assume user has been investing for ~3-6 months before using this platform
            bool hasInvestmentHistory = investmentType != "none" && investmentType != "";
            decimal assumedMonths = hasInvestmentHistory ? 4 : 2;
            initialAmount += userSIPAmount * assumedMonths;
        }

        // Factor 4: If savings is 0 but SIP is significant, ensure minimum progress
        if (userSavings == 0 && userSIPAmount > 0 && initialAmount < userSIPAmount * 2)
        {
            initialAmount = Math.Max(initialAmount, userSIPAmount * 2);
        }

        // Cap at 40% — don't show unrealistically high progress for new goals
        decimal maxAllowed = targetAmount * 0.40m;
        initialAmount = Math.Min(initialAmount, maxAllowed);

        // Round to nearest 1000
        initialAmount = Math.Round(initialAmount / 1000) * 1000;

        return initialAmount;
    }

    [HttpPost("recalculate")]
    public async Task<IActionResult> RecalculateProgress()
    {
        var userId = GetUserId();
        var goals = await _context.Goals
            .Where(g => g.UserId == userId && g.IsActive)
            .ToListAsync();

        if (!goals.Any())
            return Ok(new { message = "No goals to recalculate" });

        // Get user's financial profile
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        decimal userSavings = profile?.Savings ?? 0;
        decimal userSIPAmount = profile?.SIPAmount ?? 0;
        string existingInvestments = profile?.ExistingInvestments ?? "None";
        int goalCount = goals.Count;

        foreach (var goal in goals)
        {
            decimal newAmount = CalculateInitialProgress(
                goal.TargetAmount, goal.TargetYears,
                userSavings, userSIPAmount, existingInvestments, goalCount);

            // Only update if new calculation is higher (don't reduce progress)
            if (newAmount > goal.CurrentAmount)
            {
                goal.CurrentAmount = newAmount;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Goals recalculated", count = goals.Count });
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

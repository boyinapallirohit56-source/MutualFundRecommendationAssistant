using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class AdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    // --- User Management ---

    public async Task<List<AdminUserDTO>> GetAllUsers()
    {
        return await _context.Users
            .Where(u => u.Role != "Admin")
            .Select(u => new AdminUserDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                HasProfile = u.Profile != null,
                HasAssessment = u.RiskAssessments.Any(),
                RiskProfile = u.RiskAssessments
                    .OrderByDescending(a => a.CompletedAt)
                    .Select(a => a.RiskProfile)
                    .FirstOrDefault()
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateUserStatus(int userId, bool isActive)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    // --- Questionnaire Management ---

    public async Task<RiskQuestion> AddQuestion(AdminQuestionDTO dto)
    {
        var question = new RiskQuestion
        {
            QuestionText = dto.QuestionText,
            OrderNumber = dto.OrderNumber,
            Options = dto.Options.Select(o => new RiskOption
            {
                OptionText = o.OptionText,
                Score = o.Score
            }).ToList()
        };

        _context.RiskQuestions.Add(question);
        await _context.SaveChangesAsync();
        return question;
    }

    public async Task<bool> UpdateQuestion(int questionId, AdminQuestionDTO dto)
    {
        var question = await _context.RiskQuestions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null) return false;

        question.QuestionText = dto.QuestionText;
        question.OrderNumber = dto.OrderNumber;

        // Remove old options and add new ones
        _context.RiskOptions.RemoveRange(question.Options);
        question.Options = dto.Options.Select(o => new RiskOption
        {
            QuestionId = questionId,
            OptionText = o.OptionText,
            Score = o.Score
        }).ToList();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteQuestion(int questionId)
    {
        var question = await _context.RiskQuestions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null) return false;

        question.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // --- Fund Management ---

    public async Task<MutualFund> AddFund(AdminFundDTO dto)
    {
        var fund = new MutualFund
        {
            Name = dto.Name,
            Category = dto.Category,
            SubCategory = dto.SubCategory,
            AMC = dto.AMC,
            NAV = dto.NAV,
            ExpenseRatio = dto.ExpenseRatio,
            CAGR1Y = dto.CAGR1Y,
            CAGR3Y = dto.CAGR3Y,
            CAGR5Y = dto.CAGR5Y,
            AUM = dto.AUM,
            FundManager = dto.FundManager,
            Rating = dto.Rating
        };

        _context.MutualFunds.Add(fund);
        await _context.SaveChangesAsync();
        return fund;
    }

    public async Task<bool> UpdateFund(int fundId, AdminFundDTO dto)
    {
        var fund = await _context.MutualFunds.FindAsync(fundId);
        if (fund == null) return false;

        fund.Name = dto.Name;
        fund.Category = dto.Category;
        fund.SubCategory = dto.SubCategory;
        fund.AMC = dto.AMC;
        fund.NAV = dto.NAV;
        fund.ExpenseRatio = dto.ExpenseRatio;
        fund.CAGR1Y = dto.CAGR1Y;
        fund.CAGR3Y = dto.CAGR3Y;
        fund.CAGR5Y = dto.CAGR5Y;
        fund.AUM = dto.AUM;
        fund.FundManager = dto.FundManager;
        fund.Rating = dto.Rating;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteFund(int fundId)
    {
        var fund = await _context.MutualFunds.FindAsync(fundId);
        if (fund == null) return false;

        fund.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateFund(int fundId)
    {
        var fund = await _context.MutualFunds.FindAsync(fundId);
        if (fund == null) return false;

        fund.IsActive = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // --- Analytics ---

    public async Task<AdminAnalyticsDTO> GetAnalytics()
    {
        var totalUsers = await _context.Users.CountAsync(u => u.Role == "User");
        var activeUsers = await _context.Users.CountAsync(u => u.Role == "User" && u.IsActive);
        var totalAssessments = await _context.RiskAssessments.CountAsync();
        var totalRecommendations = await _context.Recommendations.CountAsync();

        // Risk profile distribution
        var riskDistribution = await _context.RiskAssessments
            .GroupBy(a => a.RiskProfile)
            .Select(g => new { Profile = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Profile, x => x.Count);

        // Goal distribution
        var profiles = await _context.UserProfiles
            .Where(p => !string.IsNullOrEmpty(p.Goals))
            .Select(p => p.Goals)
            .ToListAsync();

        var goalDistribution = profiles
            .SelectMany(g => g.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(g => g.Trim())
            .ToDictionary(g => g.Key, g => g.Count());

        // Recent activity
        var recentAssessments = await _context.RiskAssessments
            .Include(a => a.User)
            .OrderByDescending(a => a.CompletedAt)
            .Take(10)
            .Select(a => new RecentActivityDTO
            {
                UserName = a.User.Name,
                Action = $"Completed risk assessment ({a.RiskProfile})",
                Timestamp = a.CompletedAt
            })
            .ToListAsync();

        return new AdminAnalyticsDTO
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalAssessments = totalAssessments,
            TotalRecommendations = totalRecommendations,
            RiskProfileDistribution = riskDistribution,
            GoalDistribution = goalDistribution,
            RecentActivity = recentAssessments
        };
    }

    // --- Allocation Rules ---

    public async Task<List<AllocationRule>> GetAllocationRules()
    {
        return await _context.AllocationRules
            .OrderBy(r => r.RiskProfile)
            .ThenBy(r => r.AssetClass)
            .ToListAsync();
    }

    public async Task<bool> UpdateAllocationRules(string riskProfile, List<AllocationRuleItemDTO> allocations)
    {
        var existingRules = await _context.AllocationRules
            .Where(r => r.RiskProfile == riskProfile)
            .ToListAsync();

        if (!existingRules.Any()) return false;

        foreach (var rule in existingRules)
        {
            var updated = allocations.FirstOrDefault(a => a.AssetClass == rule.AssetClass);
            if (updated != null)
            {
                rule.Percentage = updated.Percentage;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
}

using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class RecommendationService
{
    private readonly AppDbContext _context;
    private readonly NotificationService _notificationService;

    public RecommendationService(AppDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<RecommendationResponseDTO?> GenerateRecommendation(int userId)
    {
        // Get latest risk assessment
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        if (assessment == null) return null;

        // Get allocation rules for this risk profile
        var rules = await _context.AllocationRules
            .Where(r => r.RiskProfile == assessment.RiskProfile)
            .ToListAsync();

        if (!rules.Any()) return null;

        // Create recommendation
        var recommendation = new Recommendation
        {
            UserId = userId,
            RiskAssessmentId = assessment.Id,
            RiskProfile = assessment.RiskProfile,
            GeneratedAt = DateTime.UtcNow,
            AIExplanation = GenerateExplanation(assessment.RiskProfile)
        };

        _context.Recommendations.Add(recommendation);
        await _context.SaveChangesAsync();

        // Auto-trigger notification
        await _notificationService.CreateNotification(userId,
            "New Recommendation Generated",
            $"Your {assessment.RiskProfile} allocation has been prepared with fund suggestions. View it on your dashboard.",
            "recommendation");

        // Create allocations with suggested funds
        foreach (var rule in rules.Where(r => r.Percentage > 0))
        {
            var suggestedFunds = await GetTopFunds(rule.AssetClass, 2);

            _context.RecommendationAllocations.Add(new RecommendationAllocation
            {
                RecommendationId = recommendation.Id,
                AssetClass = rule.AssetClass,
                Percentage = rule.Percentage,
                SuggestedFunds = string.Join(", ", suggestedFunds.Select(f => f.Name))
            });
        }
        await _context.SaveChangesAsync();

        return await GetRecommendation(recommendation.Id);
    }

    public async Task<RecommendationResponseDTO?> GetLatestRecommendation(int userId)
    {
        var recommendation = await _context.Recommendations
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .FirstOrDefaultAsync();

        if (recommendation == null) return null;

        return await GetRecommendation(recommendation.Id);
    }

    private async Task<RecommendationResponseDTO?> GetRecommendation(int recommendationId)
    {
        var recommendation = await _context.Recommendations
            .Include(r => r.Allocations)
            .FirstOrDefaultAsync(r => r.Id == recommendationId);

        if (recommendation == null) return null;

        return new RecommendationResponseDTO
        {
            Id = recommendation.Id,
            RiskProfile = recommendation.RiskProfile,
            AIExplanation = recommendation.AIExplanation,
            GeneratedAt = recommendation.GeneratedAt,
            Allocations = recommendation.Allocations.Select(a => new AllocationDTO
            {
                AssetClass = a.AssetClass,
                Percentage = a.Percentage,
                SuggestedFunds = a.SuggestedFunds
            }).ToList()
        };
    }

    private async Task<List<MutualFund>> GetTopFunds(string category, int count)
    {
        return await _context.MutualFunds
            .Where(f => f.Category == category && f.IsActive)
            .OrderByDescending(f => f.Rating)
            .ThenByDescending(f => f.CAGR3Y)
            .ThenBy(f => f.ExpenseRatio)
            .Take(count)
            .ToListAsync();
    }

    private static string GenerateExplanation(string riskProfile)
    {
        return riskProfile switch
        {
            "Conservative" => "Based on your risk assessment, we recommend a conservative allocation focused primarily on debt instruments and fixed-income funds. This approach prioritizes capital preservation while still providing moderate growth through a small equity component. Your portfolio is designed to minimize volatility and provide steady returns.",

            "Moderate" => "Your risk profile suggests a balanced approach. We've allocated a significant portion to equity for growth while maintaining stability through debt and hybrid funds. This mix aims to provide better-than-inflation returns while keeping risk at manageable levels. The diversification across asset classes helps reduce overall portfolio volatility.",

            "Aggressive" => "Your assessment shows you're comfortable with market volatility for higher growth potential. The recommended allocation is equity-heavy, targeting long-term wealth creation. A small portion in debt and gold provides some downside protection. This allocation is suitable for your investment horizon and risk tolerance.",

            "Very Aggressive" => "You have a high risk tolerance and a growth-focused approach. The allocation maximizes equity exposure across large, mid, and small-cap funds for maximum growth potential. International equity adds geographical diversification. This portfolio may experience significant short-term volatility but is positioned for strong long-term returns.",

            _ => "Recommendation generated based on your risk profile."
        };
    }
}

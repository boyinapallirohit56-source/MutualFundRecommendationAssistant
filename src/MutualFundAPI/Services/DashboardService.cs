using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

public class DashboardService
{
    private readonly AppDbContext _context;
    private readonly PortfolioService _portfolioService;
    private readonly RiskAssessmentService _assessmentService;
    private readonly RecommendationService _recommendationService;

    public DashboardService(
        AppDbContext context,
        PortfolioService portfolioService,
        RiskAssessmentService assessmentService,
        RecommendationService recommendationService)
    {
        _context = context;
        _portfolioService = portfolioService;
        _assessmentService = assessmentService;
        _recommendationService = recommendationService;
    }

    public async Task<DashboardDTO> GetDashboard(int userId)
    {
        var dashboard = new DashboardDTO();

        // Risk Assessment
        var assessment = await _assessmentService.GetLatestAssessment(userId);
        if (assessment != null)
        {
            dashboard.RiskScore = assessment.NormalizedScore;
            dashboard.RiskProfile = assessment.RiskProfile;
            dashboard.AssessmentDate = assessment.CompletedAt;
        }

        // Recommendation / Allocation
        var recommendation = await _recommendationService.GetLatestRecommendation(userId);
        if (recommendation != null)
        {
            dashboard.Allocations = recommendation.Allocations;
            dashboard.AIExplanation = recommendation.AIExplanation;
        }

        // Portfolio Summary
        var portfolio = await _portfolioService.GetPortfolio(userId);
        if (portfolio != null)
        {
            dashboard.PortfolioSummary = new PortfolioSummaryBriefDTO
            {
                TotalInvested = portfolio.TotalInvested,
                CurrentValue = portfolio.CurrentValue,
                TotalReturns = portfolio.TotalReturns,
                ReturnsPercentage = portfolio.ReturnsPercentage,
                TotalHoldings = portfolio.TotalHoldings
            };
        }

        // Goal Progress
        var goals = await _context.Goals
            .Where(g => g.UserId == userId && g.IsActive)
            .Select(g => new GoalProgressDTO
            {
                Name = g.Name,
                TargetAmount = g.TargetAmount,
                CurrentAmount = g.CurrentAmount,
                ProgressPercentage = g.TargetAmount > 0 ? Math.Round((g.CurrentAmount / g.TargetAmount) * 100, 1) : 0
            })
            .ToListAsync();
        dashboard.Goals = goals;

        // SIP Info
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile != null && profile.SIPAmount > 0)
        {
            dashboard.SIPAmount = profile.SIPAmount;
            dashboard.UpcomingSIPDates = GetUpcomingSIPDates(3);
        }

        // Recent Activity
        dashboard.RecentActivity = await GetRecentActivity(userId);

        // Notification Count
        var unreadNotifications = await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
        dashboard.UnreadNotifications = unreadNotifications;

        return dashboard;
    }

    private List<string> GetUpcomingSIPDates(int count)
    {
        var dates = new List<string>();
        var today = DateTime.UtcNow;
        for (int i = 0; i < count; i++)
        {
            var sipDate = new DateTime(today.Year, today.Month, 5).AddMonths(i);
            if (sipDate <= today) sipDate = sipDate.AddMonths(1);
            dates.Add(sipDate.ToString("dd MMM yyyy"));
        }
        return dates;
    }

    private async Task<List<ActivityDTO>> GetRecentActivity(int userId)
    {
        var activities = new List<ActivityDTO>();

        // Latest assessment
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();
        if (assessment != null)
        {
            activities.Add(new ActivityDTO
            {
                Text = $"Completed risk assessment ({assessment.RiskProfile})",
                Timestamp = assessment.CompletedAt,
                Type = "assessment"
            });
        }

        // Latest recommendation
        var recommendation = await _context.Recommendations
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.GeneratedAt)
            .FirstOrDefaultAsync();
        if (recommendation != null)
        {
            activities.Add(new ActivityDTO
            {
                Text = $"Received fund recommendation ({recommendation.RiskProfile} profile)",
                Timestamp = recommendation.GeneratedAt,
                Type = "recommendation"
            });
        }

        // Latest portfolio update
        var latestHolding = await _context.PortfolioHoldings
            .Include(h => h.Portfolio)
            .Where(h => h.Portfolio.UserId == userId)
            .OrderByDescending(h => h.PurchaseDate)
            .FirstOrDefaultAsync();
        if (latestHolding != null)
        {
            activities.Add(new ActivityDTO
            {
                Text = $"Added {latestHolding.FundName} to portfolio",
                Timestamp = latestHolding.PurchaseDate,
                Type = "portfolio"
            });
        }

        return activities.OrderByDescending(a => a.Timestamp).Take(5).ToList();
    }
}

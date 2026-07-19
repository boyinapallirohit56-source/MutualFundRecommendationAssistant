using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

public class ReportService
{
    private readonly AppDbContext _context;
    private readonly PortfolioService _portfolioService;
    private readonly RecommendationService _recommendationService;
    private readonly RiskAssessmentService _assessmentService;

    public ReportService(
        AppDbContext context,
        PortfolioService portfolioService,
        RecommendationService recommendationService,
        RiskAssessmentService assessmentService)
    {
        _context = context;
        _portfolioService = portfolioService;
        _recommendationService = recommendationService;
        _assessmentService = assessmentService;
    }

    public async Task<RiskAssessmentReportDTO?> GenerateRiskAssessmentReport(int userId)
    {
        var assessment = await _context.RiskAssessments
            .Include(a => a.Responses)
            .ThenInclude(r => r.Question)
            .Include(a => a.Responses)
            .ThenInclude(r => r.SelectedOption)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        if (assessment == null) return null;

        var user = await _context.Users.FindAsync(userId);
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        var responseDetails = assessment.Responses
            .OrderBy(r => r.Question.OrderNumber)
            .Select(r => new QuestionResponseDetail
            {
                Question = r.Question.QuestionText,
                SelectedAnswer = r.SelectedOption.OptionText,
                Score = r.SelectedOption.Score,
                MaxScore = 4
            })
            .ToList();

        return new RiskAssessmentReportDTO
        {
            ReportTitle = "Risk Assessment Report",
            GeneratedAt = DateTime.UtcNow,
            UserName = user?.Name ?? "Unknown",
            UserEmail = user?.Email ?? "Unknown",
            Age = profile?.Age ?? 0,
            Occupation = profile?.Occupation ?? "Not specified",
            RiskScore = assessment.TotalScore,
            RiskProfile = assessment.RiskProfile,
            ProfileDescription = GetProfileDescription(assessment.RiskProfile),
            CompletedAt = assessment.CompletedAt,
            TotalQuestions = responseDetails.Count,
            TotalScore = responseDetails.Sum(r => r.Score),
            MaxPossibleScore = responseDetails.Count * 4,
            Responses = responseDetails
        };
    }

    public async Task<RecommendationReportDTO?> GenerateRecommendationReport(int userId)
    {
        var recommendation = await _recommendationService.GetLatestRecommendation(userId);
        if (recommendation == null) return null;

        var user = await _context.Users.FindAsync(userId);
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var assessment = await _assessmentService.GetLatestAssessment(userId);

        return new RecommendationReportDTO
        {
            ReportTitle = "Mutual Fund Recommendation Report",
            GeneratedAt = DateTime.UtcNow,
            UserName = user?.Name ?? "Unknown",
            RiskScore = assessment?.NormalizedScore ?? 0,
            RiskProfile = recommendation.RiskProfile,
            InvestmentDuration = profile?.DurationInYears ?? 0,
            SIPAmount = profile?.SIPAmount ?? 0,
            Goals = profile?.Goals ?? "Not specified",
            Allocations = recommendation.Allocations,
            AIExplanation = recommendation.AIExplanation ?? "",
            Disclaimer = "This report is for educational purposes only and does not constitute certified financial advice. Please consult a SEBI-registered financial advisor before making investment decisions."
        };
    }

    public async Task<PortfolioReportDTO?> GeneratePortfolioReport(int userId)
    {
        var portfolio = await _portfolioService.GetPortfolio(userId);
        if (portfolio == null) return null;

        var analysis = await _portfolioService.AnalyzePortfolio(userId);
        var user = await _context.Users.FindAsync(userId);

        return new PortfolioReportDTO
        {
            ReportTitle = "Portfolio Analysis Report",
            GeneratedAt = DateTime.UtcNow,
            UserName = user?.Name ?? "Unknown",
            PortfolioName = portfolio.Name,
            TotalInvested = portfolio.TotalInvested,
            CurrentValue = portfolio.CurrentValue,
            TotalReturns = portfolio.TotalReturns,
            ReturnsPercentage = portfolio.ReturnsPercentage,
            TotalHoldings = portfolio.TotalHoldings,
            Holdings = portfolio.Holdings,
            Analysis = analysis,
            Disclaimer = "This report is for educational purposes only. Past performance does not guarantee future results."
        };
    }

    public async Task<StressTestReportDTO> GenerateStressTestReport(int userId, StressTestRequestDTO request)
    {
        var portfolio = await _portfolioService.GetPortfolio(userId);
        var user = await _context.Users.FindAsync(userId);

        if (portfolio == null || !portfolio.Holdings.Any())
        {
            return new StressTestReportDTO
            {
                ReportTitle = "Stress Test Report",
                GeneratedAt = DateTime.UtcNow,
                UserName = user?.Name ?? "Unknown",
                Scenarios = new List<StressScenarioResultDTO>(),
                ErrorMessage = "No portfolio found. Please add holdings first."
            };
        }

        var scenarios = new List<StressScenarioResultDTO>();

        foreach (var scenario in request.Scenarios)
        {
            var impactedHoldings = portfolio.Holdings.Select(h =>
            {
                // Different asset classes respond differently to market changes
                var impactMultiplier = GetImpactMultiplier(h.Category ?? "Equity", scenario.PercentageChange);
                var impactAmount = h.CurrentValue * (impactMultiplier / 100);
                var postStressValue = h.CurrentValue + impactAmount;

                return new StressHoldingImpactDTO
                {
                    FundName = h.FundName,
                    Category = h.Category ?? "Unknown",
                    CurrentValue = h.CurrentValue,
                    ImpactAmount = impactAmount,
                    PostStressValue = postStressValue,
                    ImpactPercentage = Math.Round(impactMultiplier, 2)
                };
            }).ToList();

            var totalCurrentValue = impactedHoldings.Sum(h => h.CurrentValue);
            var totalPostStress = impactedHoldings.Sum(h => h.PostStressValue);
            var totalImpact = totalPostStress - totalCurrentValue;

            scenarios.Add(new StressScenarioResultDTO
            {
                ScenarioName = scenario.Name,
                MarketChange = scenario.PercentageChange,
                PortfolioCurrentValue = totalCurrentValue,
                PortfolioPostStressValue = totalPostStress,
                PortfolioImpact = totalImpact,
                PortfolioImpactPercentage = totalCurrentValue > 0
                    ? Math.Round((totalImpact / totalCurrentValue) * 100, 2)
                    : 0,
                EstimatedRecoveryMonths = EstimateRecovery(scenario.PercentageChange),
                HoldingImpacts = impactedHoldings
            });
        }

        return new StressTestReportDTO
        {
            ReportTitle = "Stress Test Report",
            GeneratedAt = DateTime.UtcNow,
            UserName = user?.Name ?? "Unknown",
            Scenarios = scenarios,
            Disclaimer = "Stress test results are simulations based on historical patterns. Actual market behavior may differ significantly."
        };
    }

    // --- Private Helpers ---

    private static decimal GetImpactMultiplier(string category, decimal marketChange)
    {
        // Different asset classes have different sensitivity to market movements
        var beta = category switch
        {
            "Equity" => 1.2m,       // Equity moves more than market
            "Debt" => 0.2m,         // Debt barely moves
            "Hybrid" => 0.7m,       // Hybrid is in between
            "Gold" => -0.3m,        // Gold often moves opposite to market
            "Liquid" => 0.05m,      // Liquid barely affected
            "International" => 0.9m, // International equity moves with global markets
            _ => 1.0m
        };

        return marketChange * beta;
    }

    private static int EstimateRecovery(decimal marketChange)
    {
        // Rough estimate based on historical data
        return Math.Abs(marketChange) switch
        {
            <= 10 => 3,
            <= 20 => 8,
            <= 30 => 14,
            <= 40 => 24,
            _ => 36
        };
    }

    private static string GetProfileDescription(string profile)
    {
        return profile switch
        {
            "Conservative" => "You prefer capital preservation with low risk. Your investments should focus on debt and fixed-income instruments with a small allocation to equity for growth.",
            "Moderate" => "You take a balanced approach to investing. A mix of equity and debt suits your risk tolerance, providing growth potential with reasonable stability.",
            "Aggressive" => "You are growth-oriented and comfortable with market volatility. A significant equity allocation with some diversification into other asset classes is recommended.",
            "Very Aggressive" => "You seek maximum returns and can tolerate significant short-term losses. A heavily equity-focused portfolio with international diversification is suitable.",
            _ => "Risk profile not determined."
        };
    }
}

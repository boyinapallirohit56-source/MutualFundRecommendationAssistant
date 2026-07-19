using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;
using MutualFundAPI.Models.Entities;

namespace MutualFundAPI.Services;

public class PortfolioService
{
    private readonly AppDbContext _context;

    public PortfolioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PortfolioSummaryDTO?> GetPortfolio(int userId)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.Holdings)
            .ThenInclude(h => h.MutualFund)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (portfolio == null) return null;

        var holdings = portfolio.Holdings.Select(h =>
        {
            var currentNAV = h.MutualFund?.NAV ?? h.PurchaseNAV;
            var currentValue = h.Units * currentNAV;
            var returns = currentValue - h.InvestedAmount;
            var returnsPct = h.InvestedAmount > 0 ? (returns / h.InvestedAmount) * 100 : 0;

            return new HoldingDTO
            {
                Id = h.Id,
                FundName = h.FundName,
                Category = h.MutualFund?.Category,
                Units = h.Units,
                PurchaseNAV = h.PurchaseNAV,
                CurrentNAV = currentNAV,
                InvestedAmount = h.InvestedAmount,
                CurrentValue = currentValue,
                Returns = returns,
                ReturnsPercentage = Math.Round(returnsPct, 2),
                PurchaseDate = h.PurchaseDate
            };
        }).ToList();

        var totalInvested = holdings.Sum(h => h.InvestedAmount);
        var currentValue = holdings.Sum(h => h.CurrentValue);
        var totalReturns = currentValue - totalInvested;
        var returnsPct2 = totalInvested > 0 ? (totalReturns / totalInvested) * 100 : 0;

        return new PortfolioSummaryDTO
        {
            PortfolioId = portfolio.Id,
            Name = portfolio.Name,
            TotalInvested = totalInvested,
            CurrentValue = currentValue,
            TotalReturns = totalReturns,
            ReturnsPercentage = Math.Round(returnsPct2, 2),
            TotalHoldings = holdings.Count,
            Holdings = holdings
        };
    }

    public async Task<HoldingDTO> AddHolding(int userId, AddHoldingDTO dto)
    {
        // Get or create portfolio
        var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.UserId == userId);
        if (portfolio == null)
        {
            portfolio = new Portfolio { UserId = userId };
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
        }

        var holding = new PortfolioHolding
        {
            PortfolioId = portfolio.Id,
            MutualFundId = dto.MutualFundId,
            FundName = dto.FundName,
            Units = dto.Units,
            PurchaseNAV = dto.PurchaseNAV,
            InvestedAmount = dto.InvestedAmount,
            PurchaseDate = dto.PurchaseDate
        };

        _context.PortfolioHoldings.Add(holding);
        await _context.SaveChangesAsync();

        var fund = dto.MutualFundId.HasValue
            ? await _context.MutualFunds.FindAsync(dto.MutualFundId.Value)
            : null;

        var currentNAV = fund?.NAV ?? dto.PurchaseNAV;
        var currentValue = dto.Units * currentNAV;

        return new HoldingDTO
        {
            Id = holding.Id,
            FundName = holding.FundName,
            Category = fund?.Category,
            Units = holding.Units,
            PurchaseNAV = holding.PurchaseNAV,
            CurrentNAV = currentNAV,
            InvestedAmount = holding.InvestedAmount,
            CurrentValue = currentValue,
            Returns = currentValue - holding.InvestedAmount,
            ReturnsPercentage = holding.InvestedAmount > 0
                ? Math.Round(((currentValue - holding.InvestedAmount) / holding.InvestedAmount) * 100, 2)
                : 0,
            PurchaseDate = holding.PurchaseDate
        };
    }

    public async Task<bool> RemoveHolding(int userId, int holdingId)
    {
        var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.UserId == userId);
        if (portfolio == null) return false;

        var holding = await _context.PortfolioHoldings
            .FirstOrDefaultAsync(h => h.Id == holdingId && h.PortfolioId == portfolio.Id);
        if (holding == null) return false;

        _context.PortfolioHoldings.Remove(holding);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PortfolioAnalysisDTO?> AnalyzePortfolio(int userId)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.Holdings)
            .ThenInclude(h => h.MutualFund)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (portfolio == null || !portfolio.Holdings.Any())
            return null;

        var holdings = portfolio.Holdings.ToList();
        var totalInvested = holdings.Sum(h => h.InvestedAmount);
        var currentValue = holdings.Sum(h => h.Units * (h.MutualFund?.NAV ?? h.PurchaseNAV));

        // Asset Allocation
        var assetAllocation = CalculateAssetAllocation(holdings, currentValue);

        // Risk Analysis
        var riskAnalysis = await CalculateRiskAnalysis(userId, assetAllocation);

        // Diversification
        var diversification = CalculateDiversification(holdings);

        // Fund Overlap
        var overlaps = DetectFundOverlaps(holdings);

        // Rebalancing Suggestions
        var rebalancing = await CalculateRebalancing(userId, assetAllocation);

        // Insights
        var insights = GenerateInsights(assetAllocation, diversification, riskAnalysis, holdings);

        // Portfolio Score
        var score = CalculatePortfolioScore(diversification, riskAnalysis, assetAllocation);

        return new PortfolioAnalysisDTO
        {
            PortfolioScore = score,
            TotalInvested = totalInvested,
            CurrentValue = currentValue,
            OverallReturns = totalInvested > 0 ? Math.Round(((currentValue - totalInvested) / totalInvested) * 100, 2) : 0,
            AssetAllocation = assetAllocation,
            RiskAnalysis = riskAnalysis,
            Diversification = diversification,
            FundOverlaps = overlaps,
            RebalancingSuggestions = rebalancing,
            Insights = insights
        };
    }

    private List<AssetAllocationDTO> CalculateAssetAllocation(List<PortfolioHolding> holdings, decimal totalValue)
    {
        var grouped = holdings
            .GroupBy(h => h.MutualFund?.Category ?? "Unknown")
            .Select(g =>
            {
                var amount = g.Sum(h => h.Units * (h.MutualFund?.NAV ?? h.PurchaseNAV));
                return new AssetAllocationDTO
                {
                    AssetClass = g.Key,
                    Amount = amount,
                    Percentage = totalValue > 0 ? Math.Round((amount / totalValue) * 100, 1) : 0,
                    FundCount = g.Count()
                };
            })
            .OrderByDescending(a => a.Percentage)
            .ToList();

        return grouped;
    }

    private async Task<RiskAnalysisDTO> CalculateRiskAnalysis(int userId, List<AssetAllocationDTO> allocation)
    {
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        var equityPct = allocation.Where(a => a.AssetClass == "Equity").Sum(a => a.Percentage);
        var portfolioRisk = equityPct switch
        {
            >= 70 => "Very Aggressive",
            >= 50 => "Aggressive",
            >= 30 => "Moderate",
            _ => "Conservative"
        };

        var userProfile = assessment?.RiskProfile ?? "Not assessed";
        var isAligned = portfolioRisk == userProfile;

        var explanation = isAligned
            ? $"Your portfolio risk level ({portfolioRisk}) is well-aligned with your risk profile ({userProfile})."
            : $"Your portfolio is {portfolioRisk} but your risk profile is {userProfile}. Consider rebalancing to better match your risk tolerance.";

        return new RiskAnalysisDTO
        {
            PortfolioRiskLevel = portfolioRisk,
            UserRiskProfile = userProfile,
            IsAligned = isAligned,
            Explanation = explanation
        };
    }

    private DiversificationDTO CalculateDiversification(List<PortfolioHolding> holdings)
    {
        var uniqueCategories = holdings.Select(h => h.MutualFund?.Category ?? "Unknown").Distinct().Count();
        var uniqueFunds = holdings.Select(h => h.FundName).Distinct().Count();
        var uniqueAMCs = holdings.Where(h => h.MutualFund != null).Select(h => h.MutualFund!.AMC).Distinct().Count();

        // Score based on diversification factors
        int score = 0;
        score += Math.Min(uniqueCategories * 15, 40); // Max 40 points for category spread
        score += Math.Min(uniqueFunds * 8, 30);       // Max 30 points for fund variety
        score += Math.Min(uniqueAMCs * 10, 30);       // Max 30 points for AMC variety

        score = Math.Min(score, 100);

        var rating = score switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            _ => "Poor"
        };

        return new DiversificationDTO
        {
            Score = score,
            Rating = rating,
            UniqueCategories = uniqueCategories,
            UniqueFunds = uniqueFunds,
            UniqueAMCs = uniqueAMCs
        };
    }

    private List<FundOverlapDTO> DetectFundOverlaps(List<PortfolioHolding> holdings)
    {
        var overlaps = new List<FundOverlapDTO>();
        var fundsByCategory = holdings
            .Where(h => h.MutualFund != null)
            .GroupBy(h => h.MutualFund!.SubCategory)
            .Where(g => g.Count() > 1);

        foreach (var group in fundsByCategory)
        {
            var funds = group.ToList();
            for (int i = 0; i < funds.Count - 1; i++)
            {
                for (int j = i + 1; j < funds.Count; j++)
                {
                    overlaps.Add(new FundOverlapDTO
                    {
                        Fund1 = funds[i].FundName,
                        Fund2 = funds[j].FundName,
                        OverlapReason = $"Both are {group.Key} funds and likely hold similar stocks"
                    });
                }
            }
        }

        return overlaps;
    }

    private async Task<List<RebalancingSuggestionDTO>> CalculateRebalancing(int userId, List<AssetAllocationDTO> currentAllocation)
    {
        var assessment = await _context.RiskAssessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync();

        if (assessment == null) return new List<RebalancingSuggestionDTO>();

        var targetRules = await _context.AllocationRules
            .Where(r => r.RiskProfile == assessment.RiskProfile)
            .ToListAsync();

        var suggestions = new List<RebalancingSuggestionDTO>();

        foreach (var target in targetRules.Where(t => t.Percentage > 0))
        {
            var current = currentAllocation.FirstOrDefault(a => a.AssetClass == target.AssetClass);
            var currentPct = current?.Percentage ?? 0;
            var diff = target.Percentage - currentPct;

            if (Math.Abs(diff) > 5) // Only suggest if difference is significant (>5%)
            {
                suggestions.Add(new RebalancingSuggestionDTO
                {
                    AssetClass = target.AssetClass,
                    CurrentPercentage = currentPct,
                    TargetPercentage = target.Percentage,
                    Action = diff > 0 ? "Increase" : "Decrease",
                    DifferencePercentage = Math.Abs(Math.Round(diff, 1))
                });
            }
        }

        return suggestions;
    }

    private List<string> GenerateInsights(
        List<AssetAllocationDTO> allocation,
        DiversificationDTO diversification,
        RiskAnalysisDTO riskAnalysis,
        List<PortfolioHolding> holdings)
    {
        var insights = new List<string>();

        // Diversification insight
        if (diversification.Score < 40)
            insights.Add("Your portfolio is not well-diversified. Consider adding funds from different categories to reduce risk.");
        else if (diversification.Score >= 80)
            insights.Add("Great diversification! Your portfolio is spread across multiple categories and fund houses.");

        // Risk alignment
        if (!riskAnalysis.IsAligned)
            insights.Add($"Portfolio risk mismatch: Your portfolio is {riskAnalysis.PortfolioRiskLevel} but your profile is {riskAnalysis.UserRiskProfile}. Consider rebalancing.");

        // Concentration check
        var topAllocation = allocation.FirstOrDefault();
        if (topAllocation != null && topAllocation.Percentage > 70)
            insights.Add($"High concentration alert: {topAllocation.Percentage}% of your portfolio is in {topAllocation.AssetClass}. Consider diversifying.");

        // Fund count
        if (holdings.Count < 3)
            insights.Add("You have very few funds. Adding 3-5 funds across different categories can improve diversification.");
        else if (holdings.Count > 15)
            insights.Add("You have many funds which may lead to over-diversification and fund overlap. Consider consolidating.");

        // Single AMC check
        if (diversification.UniqueAMCs == 1 && holdings.Count > 2)
            insights.Add("All your funds are from the same fund house. Consider diversifying across different AMCs.");

        if (!insights.Any())
            insights.Add("Your portfolio looks well-balanced. Keep monitoring periodically.");

        return insights;
    }

    private int CalculatePortfolioScore(
        DiversificationDTO diversification,
        RiskAnalysisDTO riskAnalysis,
        List<AssetAllocationDTO> allocation)
    {
        int score = 0;

        // Diversification contributes 40%
        score += (int)(diversification.Score * 0.4);

        // Risk alignment contributes 30%
        score += riskAnalysis.IsAligned ? 30 : 15;

        // Allocation balance contributes 30%
        var topPct = allocation.FirstOrDefault()?.Percentage ?? 100;
        if (topPct <= 40) score += 30;
        else if (topPct <= 60) score += 20;
        else if (topPct <= 80) score += 10;

        return Math.Min(score, 100);
    }
}

using Microsoft.EntityFrameworkCore;
using MutualFundAPI.Data;
using MutualFundAPI.Models.DTOs;

namespace MutualFundAPI.Services;

public class FundService
{
    private readonly AppDbContext _context;

    public FundService(AppDbContext context)
    {
        _context = context;
    }

    // --- Fund Comparison (up to 4 funds) ---

    public async Task<FundComparisonDTO> CompareFunds(List<int> fundIds)
    {
        if (fundIds.Count < 2 || fundIds.Count > 4)
            throw new ArgumentException("Please select 2 to 4 funds for comparison.");

        var funds = await _context.MutualFunds
            .Where(f => fundIds.Contains(f.Id) && f.IsActive)
            .ToListAsync();

        if (funds.Count < 2)
            throw new ArgumentException("Not enough valid funds found for comparison.");

        var comparison = new FundComparisonDTO
        {
            Funds = funds.Select(f => new FundComparisonItemDTO
            {
                Id = f.Id,
                Name = f.Name,
                Category = f.Category,
                SubCategory = f.SubCategory,
                AMC = f.AMC,
                NAV = f.NAV,
                ExpenseRatio = f.ExpenseRatio,
                CAGR1Y = f.CAGR1Y,
                CAGR3Y = f.CAGR3Y,
                CAGR5Y = f.CAGR5Y,
                AUM = f.AUM,
                FundManager = f.FundManager,
                Rating = f.Rating
            }).ToList()
        };

        // Determine winners for each metric
        comparison.MetricWinners = DetermineWinners(comparison.Funds);

        return comparison;
    }

    // --- Fund Factsheet (single fund detail) ---

    public async Task<FundFactsheetDTO?> GetFundFactsheet(int fundId)
    {
        var fund = await _context.MutualFunds.FindAsync(fundId);
        if (fund == null) return null;

        // Get peer funds (same subcategory)
        var peers = await _context.MutualFunds
            .Where(f => f.SubCategory == fund.SubCategory && f.Id != fund.Id && f.IsActive)
            .OrderByDescending(f => f.Rating)
            .Take(3)
            .ToListAsync();

        // Calculate category average
        var categoryFunds = await _context.MutualFunds
            .Where(f => f.SubCategory == fund.SubCategory && f.IsActive)
            .ToListAsync();

        var avgCAGR1Y = categoryFunds.Where(f => f.CAGR1Y > 0).Average(f => f.CAGR1Y);
        var avgCAGR3Y = categoryFunds.Where(f => f.CAGR3Y > 0).Average(f => f.CAGR3Y);
        var avgExpenseRatio = categoryFunds.Where(f => f.ExpenseRatio.HasValue).Average(f => f.ExpenseRatio!.Value);

        // Calculate rank within category
        var rankByCAGR3Y = categoryFunds
            .Where(f => f.CAGR3Y > 0)
            .OrderByDescending(f => f.CAGR3Y)
            .ToList()
            .FindIndex(f => f.Id == fund.Id) + 1;

        return new FundFactsheetDTO
        {
            Id = fund.Id,
            Name = fund.Name,
            Category = fund.Category,
            SubCategory = fund.SubCategory,
            AMC = fund.AMC,
            NAV = fund.NAV,
            ExpenseRatio = fund.ExpenseRatio,
            CAGR1Y = fund.CAGR1Y,
            CAGR3Y = fund.CAGR3Y,
            CAGR5Y = fund.CAGR5Y,
            AUM = fund.AUM,
            FundManager = fund.FundManager,
            Rating = fund.Rating,
            CategoryAvgCAGR1Y = Math.Round(avgCAGR1Y, 2),
            CategoryAvgCAGR3Y = Math.Round(avgCAGR3Y, 2),
            CategoryAvgExpenseRatio = Math.Round(avgExpenseRatio, 2),
            RankInCategory = rankByCAGR3Y,
            TotalFundsInCategory = categoryFunds.Count,
            PerformanceVsBenchmark = CalculatePerformanceRating(fund, avgCAGR3Y),
            PeerFunds = peers.Select(p => new PeerFundDTO
            {
                Id = p.Id,
                Name = p.Name,
                AMC = p.AMC,
                CAGR3Y = p.CAGR3Y,
                ExpenseRatio = p.ExpenseRatio,
                Rating = p.Rating
            }).ToList()
        };
    }

    // --- List & Search Funds ---

    public async Task<List<FundListItemDTO>> ListFunds(string? category = null, string? search = null)
    {
        var query = _context.MutualFunds.Where(f => f.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(f => f.Category == category);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(f => f.Name.Contains(search) || f.AMC.Contains(search));

        return await query
            .OrderByDescending(f => f.Rating)
            .ThenByDescending(f => f.CAGR3Y)
            .Select(f => new FundListItemDTO
            {
                Id = f.Id,
                Name = f.Name,
                Category = f.Category,
                SubCategory = f.SubCategory,
                AMC = f.AMC,
                NAV = f.NAV,
                CAGR3Y = f.CAGR3Y,
                ExpenseRatio = f.ExpenseRatio,
                Rating = f.Rating
            })
            .ToListAsync();
    }

    // --- Private Helpers ---

    private static Dictionary<string, string> DetermineWinners(List<FundComparisonItemDTO> funds)
    {
        var winners = new Dictionary<string, string>();

        // Best CAGR 1Y
        var best1Y = funds.Where(f => f.CAGR1Y > 0).OrderByDescending(f => f.CAGR1Y).FirstOrDefault();
        if (best1Y != null) winners["CAGR1Y"] = best1Y.Name;

        // Best CAGR 3Y
        var best3Y = funds.Where(f => f.CAGR3Y > 0).OrderByDescending(f => f.CAGR3Y).FirstOrDefault();
        if (best3Y != null) winners["CAGR3Y"] = best3Y.Name;

        // Best CAGR 5Y
        var best5Y = funds.Where(f => f.CAGR5Y > 0).OrderByDescending(f => f.CAGR5Y).FirstOrDefault();
        if (best5Y != null) winners["CAGR5Y"] = best5Y.Name;

        // Lowest Expense Ratio (lower is better)
        var lowestExpense = funds.Where(f => f.ExpenseRatio.HasValue).OrderBy(f => f.ExpenseRatio).FirstOrDefault();
        if (lowestExpense != null) winners["ExpenseRatio"] = lowestExpense.Name;

        // Highest AUM
        var highestAUM = funds.Where(f => f.AUM.HasValue).OrderByDescending(f => f.AUM).FirstOrDefault();
        if (highestAUM != null) winners["AUM"] = highestAUM.Name;

        // Highest Rating
        var highestRating = funds.Where(f => f.Rating.HasValue).OrderByDescending(f => f.Rating).FirstOrDefault();
        if (highestRating != null) winners["Rating"] = highestRating.Name;

        return winners;
    }

    private static string CalculatePerformanceRating(Models.Entities.MutualFund fund, decimal categoryAvg)
    {
        if (fund.CAGR3Y == 0) return "Insufficient data";

        var diff = fund.CAGR3Y - categoryAvg;
        return diff switch
        {
            > 3 => "Significantly outperforming category average",
            > 1 => "Outperforming category average",
            > -1 => "In line with category average",
            > -3 => "Underperforming category average",
            _ => "Significantly underperforming category average"
        };
    }
}

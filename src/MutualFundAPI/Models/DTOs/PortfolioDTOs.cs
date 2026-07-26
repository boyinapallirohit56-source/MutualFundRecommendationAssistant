using System.ComponentModel.DataAnnotations;

namespace MutualFundAPI.Models.DTOs;

public class AddHoldingDTO
{
    public string FundName { get; set; } = string.Empty;
    public int? MutualFundId { get; set; }

    [Range(0.001, double.MaxValue, ErrorMessage = "Units must be greater than 0")]
    public decimal Units { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Purchase NAV must be greater than 0")]
    public decimal PurchaseNAV { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Invested Amount must be greater than 0")]
    public decimal InvestedAmount { get; set; }

    public DateTime PurchaseDate { get; set; }
}

public class PortfolioSummaryDTO
{
    public int PortfolioId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal ReturnsPercentage { get; set; }
    public int TotalHoldings { get; set; }
    public List<HoldingDTO> Holdings { get; set; } = new();
}

public class HoldingDTO
{
    public int Id { get; set; }
    public string FundName { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal Units { get; set; }
    public decimal PurchaseNAV { get; set; }
    public decimal? CurrentNAV { get; set; }
    public decimal InvestedAmount { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal Returns { get; set; }
    public decimal ReturnsPercentage { get; set; }
    public DateTime PurchaseDate { get; set; }
}

public class PortfolioAnalysisDTO
{
    public int PortfolioScore { get; set; } // 0-100
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal OverallReturns { get; set; }
    public List<AssetAllocationDTO> AssetAllocation { get; set; } = new();
    public RiskAnalysisDTO RiskAnalysis { get; set; } = new();
    public DiversificationDTO Diversification { get; set; } = new();
    public List<FundOverlapDTO> FundOverlaps { get; set; } = new();
    public List<string> Insights { get; set; } = new();
    public List<RebalancingSuggestionDTO> RebalancingSuggestions { get; set; } = new();
}

public class AssetAllocationDTO
{
    public string AssetClass { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public int FundCount { get; set; }
}

public class RiskAnalysisDTO
{
    public string PortfolioRiskLevel { get; set; } = string.Empty;
    public string UserRiskProfile { get; set; } = string.Empty;
    public bool IsAligned { get; set; }
    public string Explanation { get; set; } = string.Empty;
}

public class DiversificationDTO
{
    public int Score { get; set; } // 0-100
    public string Rating { get; set; } = string.Empty; // Poor, Fair, Good, Excellent
    public int UniqueCategories { get; set; }
    public int UniqueFunds { get; set; }
    public int UniqueAMCs { get; set; }
}

public class FundOverlapDTO
{
    public string Fund1 { get; set; } = string.Empty;
    public string Fund2 { get; set; } = string.Empty;
    public string OverlapReason { get; set; } = string.Empty;
}

public class RebalancingSuggestionDTO
{
    public string AssetClass { get; set; } = string.Empty;
    public decimal CurrentPercentage { get; set; }
    public decimal TargetPercentage { get; set; }
    public string Action { get; set; } = string.Empty; // "Increase" or "Decrease"
    public decimal DifferencePercentage { get; set; }
}

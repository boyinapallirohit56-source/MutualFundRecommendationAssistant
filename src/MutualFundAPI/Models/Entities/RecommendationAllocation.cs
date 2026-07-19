namespace MutualFundAPI.Models.Entities;

public class RecommendationAllocation
{
    public int Id { get; set; }
    public int RecommendationId { get; set; }
    public string AssetClass { get; set; } = string.Empty; // Equity, Debt, Hybrid, Gold, Liquid, International
    public decimal Percentage { get; set; }
    public string? SuggestedFunds { get; set; } // Comma-separated fund names

    // Navigation
    public Recommendation Recommendation { get; set; } = null!;
}

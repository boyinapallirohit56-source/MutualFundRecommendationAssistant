namespace MutualFundAPI.Models.Entities;

public class AllocationRule
{
    public int Id { get; set; }
    public string RiskProfile { get; set; } = string.Empty; // Conservative, Moderate, Aggressive, Very Aggressive
    public string AssetClass { get; set; } = string.Empty; // Equity, Debt, Hybrid, Gold, Liquid, International
    public decimal Percentage { get; set; }
}

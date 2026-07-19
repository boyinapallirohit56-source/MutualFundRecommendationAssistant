namespace MutualFundAPI.Models.Entities;

public class MutualFund
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Equity, Debt, Hybrid, Gold, Liquid, International
    public string SubCategory { get; set; } = string.Empty; // Large Cap, Mid Cap, etc.
    public string AMC { get; set; } = string.Empty; // Fund house
    public decimal? NAV { get; set; }
    public decimal? ExpenseRatio { get; set; }
    public decimal? CAGR1Y { get; set; }
    public decimal? CAGR3Y { get; set; }
    public decimal? CAGR5Y { get; set; }
    public decimal? AUM { get; set; } // in Crores
    public string? FundManager { get; set; }
    public decimal? Rating { get; set; } // 1-5 star
    public bool IsActive { get; set; } = true;
}

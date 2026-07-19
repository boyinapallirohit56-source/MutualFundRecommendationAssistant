namespace MutualFundAPI.Models.Entities;

public class PortfolioHolding
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public int? MutualFundId { get; set; }
    public string FundName { get; set; } = string.Empty;
    public decimal Units { get; set; }
    public decimal PurchaseNAV { get; set; }
    public decimal InvestedAmount { get; set; }
    public DateTime PurchaseDate { get; set; }

    // Navigation
    public Portfolio Portfolio { get; set; } = null!;
    public MutualFund? MutualFund { get; set; }
}

namespace MutualFundAPI.Models.Entities;

public class FundHolding
{
    public int Id { get; set; }
    public int MutualFundId { get; set; }
    public string StockName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public decimal Percentage { get; set; } // % of fund invested in this stock

    public MutualFund MutualFund { get; set; } = null!;
}

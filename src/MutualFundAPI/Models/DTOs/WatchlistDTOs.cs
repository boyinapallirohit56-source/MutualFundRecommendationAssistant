namespace MutualFundAPI.Models.DTOs;

public class WatchlistItemDTO
{
    public int Id { get; set; }
    public int MutualFundId { get; set; }
    public string FundName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AMC { get; set; } = string.Empty;
    public decimal? NAV { get; set; }
    public decimal? CAGR3Y { get; set; }
    public decimal? Rating { get; set; }
    public DateTime AddedAt { get; set; }
}

public class AddToWatchlistDTO
{
    public int MutualFundId { get; set; }
}

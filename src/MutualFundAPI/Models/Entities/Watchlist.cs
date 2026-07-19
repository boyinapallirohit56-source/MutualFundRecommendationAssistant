namespace MutualFundAPI.Models.Entities;

public class WatchlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MutualFundId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public MutualFund MutualFund { get; set; } = null!;
}

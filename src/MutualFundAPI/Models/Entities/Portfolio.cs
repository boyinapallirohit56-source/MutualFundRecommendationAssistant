namespace MutualFundAPI.Models.Entities;

public class Portfolio
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = "My Portfolio";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<PortfolioHolding> Holdings { get; set; } = new List<PortfolioHolding>();
}

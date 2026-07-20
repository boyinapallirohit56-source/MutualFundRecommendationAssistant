namespace MutualFundAPI.Models.Entities;

public class Goal
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty; // Wealth Creation, Retirement, etc.
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public int TargetYears { get; set; }
    public decimal MonthlySIP { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Calculated
    public decimal ProgressPercentage => TargetAmount > 0 ? Math.Round((CurrentAmount / TargetAmount) * 100, 1) : 0;

    public User User { get; set; } = null!;
}

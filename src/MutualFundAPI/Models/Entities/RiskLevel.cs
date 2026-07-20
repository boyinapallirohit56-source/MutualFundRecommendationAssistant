namespace MutualFundAPI.Models.Entities;

public class RiskLevel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Conservative, Moderate, Aggressive, Very Aggressive
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

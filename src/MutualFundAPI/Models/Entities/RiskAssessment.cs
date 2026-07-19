namespace MutualFundAPI.Models.Entities;

public class RiskAssessment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TotalScore { get; set; }
    public string RiskProfile { get; set; } = string.Empty; // Conservative, Moderate, Aggressive, Very Aggressive
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<RiskResponse> Responses { get; set; } = new List<RiskResponse>();
}

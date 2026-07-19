namespace MutualFundAPI.Models.Entities;

public class Recommendation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RiskAssessmentId { get; set; }
    public string RiskProfile { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string? AIExplanation { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public RiskAssessment RiskAssessment { get; set; } = null!;
    public ICollection<RecommendationAllocation> Allocations { get; set; } = new List<RecommendationAllocation>();
}

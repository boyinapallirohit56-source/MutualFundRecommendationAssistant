namespace MutualFundAPI.Models.Entities;

public class RiskResponse
{
    public int Id { get; set; }
    public int AssessmentId { get; set; }
    public int QuestionId { get; set; }
    public int SelectedOptionId { get; set; }

    // Navigation
    public RiskAssessment Assessment { get; set; } = null!;
    public RiskQuestion Question { get; set; } = null!;
    public RiskOption SelectedOption { get; set; } = null!;
}

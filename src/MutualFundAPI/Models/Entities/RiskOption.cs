namespace MutualFundAPI.Models.Entities;

public class RiskOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int Score { get; set; } // Weight/score for this option

    // Navigation
    public RiskQuestion Question { get; set; } = null!;
}

namespace MutualFundAPI.Models.Entities;

public class RiskQuestion
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<RiskOption> Options { get; set; } = new List<RiskOption>();
}

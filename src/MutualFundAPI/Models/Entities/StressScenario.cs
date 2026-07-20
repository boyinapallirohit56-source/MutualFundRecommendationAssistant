namespace MutualFundAPI.Models.Entities;

public class StressScenario
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PercentageChange { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

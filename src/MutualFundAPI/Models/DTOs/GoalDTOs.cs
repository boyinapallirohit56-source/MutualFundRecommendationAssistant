namespace MutualFundAPI.Models.DTOs;

public class CreateGoalDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public int TargetYears { get; set; }
    public decimal MonthlySIP { get; set; }
}

public class GoalResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public int TargetYears { get; set; }
    public decimal MonthlySIP { get; set; }
    public decimal ProgressPercentage { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateGoalProgressDTO
{
    public decimal CurrentAmount { get; set; }
}

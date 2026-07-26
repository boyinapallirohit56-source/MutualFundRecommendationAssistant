using System.ComponentModel.DataAnnotations;

namespace MutualFundAPI.Models.DTOs;

public class CreateGoalDTO
{
    public string Name { get; set; } = string.Empty;

    [Range(1, double.MaxValue, ErrorMessage = "Target Amount must be greater than 0")]
    public decimal TargetAmount { get; set; }

    [Range(1, 50, ErrorMessage = "Target Years must be between 1 and 50")]
    public int TargetYears { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Monthly SIP cannot be negative")]
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
    [Range(0, double.MaxValue, ErrorMessage = "Current Amount cannot be negative")]
    public decimal CurrentAmount { get; set; }
}

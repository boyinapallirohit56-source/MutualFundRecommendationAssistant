namespace MutualFundAPI.Models.DTOs;

public class DashboardDTO
{
    // Risk
    public int RiskScore { get; set; }
    public string? RiskProfile { get; set; }
    public DateTime? AssessmentDate { get; set; }

    // Allocation
    public List<AllocationDTO>? Allocations { get; set; }
    public string? AIExplanation { get; set; }

    // Portfolio
    public PortfolioSummaryBriefDTO? PortfolioSummary { get; set; }

    // Goals
    public List<GoalProgressDTO> Goals { get; set; } = new();

    // SIP
    public decimal SIPAmount { get; set; }
    public List<string> UpcomingSIPDates { get; set; } = new();

    // Activity
    public List<ActivityDTO> RecentActivity { get; set; } = new();

    // Notifications
    public int UnreadNotifications { get; set; }
}

public class PortfolioSummaryBriefDTO
{
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal ReturnsPercentage { get; set; }
    public int TotalHoldings { get; set; }
}

public class GoalProgressDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class ActivityDTO
{
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty;
}

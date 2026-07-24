namespace MutualFundAPI.Models.DTOs;

public class AdminUserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasProfile { get; set; }
    public bool HasAssessment { get; set; }
    public string? RiskProfile { get; set; }
}

public class AdminAnalyticsDTO
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalAssessments { get; set; }
    public int TotalRecommendations { get; set; }
    public Dictionary<string, int> RiskProfileDistribution { get; set; } = new();
    public Dictionary<string, int> GoalDistribution { get; set; } = new();
    public List<RecentActivityDTO> RecentActivity { get; set; } = new();
}

public class RecentActivityDTO
{
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class UpdateUserStatusDTO
{
    public bool IsActive { get; set; }
}

public class AdminQuestionDTO
{
    public string QuestionText { get; set; } = string.Empty;
    public int OrderNumber { get; set; }
    public List<AdminOptionDTO> Options { get; set; } = new();
}

public class AdminOptionDTO
{
    public string OptionText { get; set; } = string.Empty;
    public int Score { get; set; }
}

public class AdminFundDTO
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string AMC { get; set; } = string.Empty;
    public decimal? NAV { get; set; }
    public decimal? ExpenseRatio { get; set; }
    public decimal? CAGR1Y { get; set; }
    public decimal? CAGR3Y { get; set; }
    public decimal? CAGR5Y { get; set; }
    public decimal? AUM { get; set; }
    public string? FundManager { get; set; }
    public decimal? Rating { get; set; }
}


public class UpdateAllocationRulesDTO
{
    public List<AllocationRuleItemDTO> Allocations { get; set; } = new();
}

public class AllocationRuleItemDTO
{
    public string AssetClass { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
}

namespace MutualFundAPI.Models.DTOs;

// --- Risk Assessment Report ---
public class RiskAssessmentReportDTO
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskProfile { get; set; } = string.Empty;
    public string ProfileDescription { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public int TotalQuestions { get; set; }
    public int TotalScore { get; set; }
    public int MaxPossibleScore { get; set; }
    public List<QuestionResponseDetail> Responses { get; set; } = new();
}

public class QuestionResponseDetail
{
    public string Question { get; set; } = string.Empty;
    public string SelectedAnswer { get; set; } = string.Empty;
    public int Score { get; set; }
    public int MaxScore { get; set; }
}

// --- Recommendation Report ---
public class RecommendationReportDTO
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskProfile { get; set; } = string.Empty;
    public int InvestmentDuration { get; set; }
    public decimal SIPAmount { get; set; }
    public string Goals { get; set; } = string.Empty;
    public List<AllocationDTO> Allocations { get; set; } = new();
    public string AIExplanation { get; set; } = string.Empty;
    public string Disclaimer { get; set; } = string.Empty;
}

// --- Portfolio Report ---
public class PortfolioReportDTO
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PortfolioName { get; set; } = string.Empty;
    public decimal TotalInvested { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal ReturnsPercentage { get; set; }
    public int TotalHoldings { get; set; }
    public List<HoldingDTO> Holdings { get; set; } = new();
    public PortfolioAnalysisDTO? Analysis { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}

// --- Stress Test ---
public class StressTestRequestDTO
{
    public List<StressScenarioDTO> Scenarios { get; set; } = new();
}

public class StressScenarioDTO
{
    public string Name { get; set; } = string.Empty;
    public decimal PercentageChange { get; set; } // e.g., -10, -20, -30, +20
}

public class StressTestReportDTO
{
    public string ReportTitle { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<StressScenarioResultDTO> Scenarios { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string Disclaimer { get; set; } = string.Empty;
}

public class StressScenarioResultDTO
{
    public string ScenarioName { get; set; } = string.Empty;
    public decimal MarketChange { get; set; }
    public decimal PortfolioCurrentValue { get; set; }
    public decimal PortfolioPostStressValue { get; set; }
    public decimal PortfolioImpact { get; set; }
    public decimal PortfolioImpactPercentage { get; set; }
    public int EstimatedRecoveryMonths { get; set; }
    public List<StressHoldingImpactDTO> HoldingImpacts { get; set; } = new();
}

public class StressHoldingImpactDTO
{
    public string FundName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal ImpactAmount { get; set; }
    public decimal PostStressValue { get; set; }
    public decimal ImpactPercentage { get; set; }
}

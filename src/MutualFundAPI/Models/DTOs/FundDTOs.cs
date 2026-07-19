namespace MutualFundAPI.Models.DTOs;

// --- Fund List ---
public class FundListItemDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string AMC { get; set; } = string.Empty;
    public decimal? CAGR3Y { get; set; }
    public decimal? ExpenseRatio { get; set; }
    public decimal? Rating { get; set; }
}

// --- Fund Comparison ---
public class FundComparisonRequestDTO
{
    public List<int> FundIds { get; set; } = new();
}

public class FundComparisonDTO
{
    public List<FundComparisonItemDTO> Funds { get; set; } = new();
    public Dictionary<string, string> MetricWinners { get; set; } = new();
}

public class FundComparisonItemDTO
{
    public int Id { get; set; }
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

// --- Fund Factsheet ---
public class FundFactsheetDTO
{
    public int Id { get; set; }
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

    // Category context
    public decimal CategoryAvgCAGR1Y { get; set; }
    public decimal CategoryAvgCAGR3Y { get; set; }
    public decimal CategoryAvgExpenseRatio { get; set; }
    public int RankInCategory { get; set; }
    public int TotalFundsInCategory { get; set; }
    public string PerformanceVsBenchmark { get; set; } = string.Empty;

    // Peers
    public List<PeerFundDTO> PeerFunds { get; set; } = new();
}

public class PeerFundDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AMC { get; set; } = string.Empty;
    public decimal? CAGR3Y { get; set; }
    public decimal? ExpenseRatio { get; set; }
    public decimal? Rating { get; set; }
}

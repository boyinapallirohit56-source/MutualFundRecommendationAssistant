namespace MutualFundAPI.Models.Entities;

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Personal
    public int Age { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public int Dependents { get; set; }

    // Financial
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal Savings { get; set; }
    public decimal Loans { get; set; }

    // Investment
    public string ExistingInvestments { get; set; } = string.Empty;
    public decimal SIPAmount { get; set; }
    public string SIPFrequency { get; set; } = "Monthly"; // Weekly, Monthly, Quarterly
    public int SIPDate { get; set; } = 5; // Day of month (1-28) for Monthly, Day of week (1-7) for Weekly
    public decimal LumpSumAmount { get; set; }
    public bool HasSWP { get; set; } = false;
    public decimal SWPAmount { get; set; }
    public string InvestmentType { get; set; } = "SIP"; // SIP, LumpSum, Both
    public int DurationInYears { get; set; }

    // Goals
    public string Goals { get; set; } = string.Empty; // Comma-separated: "WealthCreation,Retirement,TaxSaving"

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}

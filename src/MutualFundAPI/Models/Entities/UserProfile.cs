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
    public int DurationInYears { get; set; }

    // Goals
    public string Goals { get; set; } = string.Empty; // Comma-separated: "WealthCreation,Retirement,TaxSaving"

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}

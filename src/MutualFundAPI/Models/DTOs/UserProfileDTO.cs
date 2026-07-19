namespace MutualFundAPI.Models.DTOs;

public class UserProfileDTO
{
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

    // Goals (comma-separated)
    public string Goals { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MutualFundAPI.Models.DTOs;

public class UserProfileDTO
{
    // Personal
    [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
    public int Age { get; set; }
    public string Occupation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;

    [Range(0, 20, ErrorMessage = "Dependents cannot be negative")]
    public int Dependents { get; set; }

    // Financial
    [Range(0, double.MaxValue, ErrorMessage = "Monthly Income cannot be negative")]
    public decimal MonthlyIncome { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Monthly Expenses cannot be negative")]
    public decimal MonthlyExpenses { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Savings cannot be negative")]
    public decimal Savings { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Loans cannot be negative")]
    public decimal Loans { get; set; }

    // Investment
    public string ExistingInvestments { get; set; } = string.Empty;
    public string InvestmentType { get; set; } = "SIP"; // SIP, LumpSum, Both

    [JsonPropertyName("sipAmount")]
    [Range(0, double.MaxValue, ErrorMessage = "SIP Amount cannot be negative")]
    public decimal SIPAmount { get; set; }

    [JsonPropertyName("sipFrequency")]
    public string SIPFrequency { get; set; } = "Monthly"; // Weekly, Monthly, Quarterly

    [JsonPropertyName("sipDate")]
    [Range(1, 31, ErrorMessage = "SIP Date must be between 1 and 31")]
    public int SIPDate { get; set; } = 5; // Day of month or day of week

    [Range(0, double.MaxValue, ErrorMessage = "Lump Sum Amount cannot be negative")]
    public decimal LumpSumAmount { get; set; }

    [JsonPropertyName("hasSWP")]
    public bool HasSWP { get; set; } = false;

    [JsonPropertyName("swpAmount")]
    [Range(0, double.MaxValue, ErrorMessage = "SWP Amount cannot be negative")]
    public decimal SWPAmount { get; set; }

    [Range(0, 50, ErrorMessage = "Duration must be between 0 and 50 years")]
    public int DurationInYears { get; set; }

    // Goals (comma-separated)
    public string Goals { get; set; } = string.Empty;
}

namespace MutualFundAPI.Models.Entities;

public class FundCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Equity, Debt, Hybrid, Gold, Liquid, International
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

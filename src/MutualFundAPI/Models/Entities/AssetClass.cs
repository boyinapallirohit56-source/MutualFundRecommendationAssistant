namespace MutualFundAPI.Models.Entities;

public class AssetClass
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Large Cap, Mid Cap, Small Cap, etc.
    public string Category { get; set; } = string.Empty; // Equity, Debt, etc.
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

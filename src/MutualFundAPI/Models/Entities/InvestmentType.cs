namespace MutualFundAPI.Models.Entities;

public class InvestmentType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // SIP, Lump Sum, etc.
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

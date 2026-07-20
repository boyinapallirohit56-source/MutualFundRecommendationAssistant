namespace MutualFundAPI.Models.Entities;

public class FundNAVHistory
{
    public int Id { get; set; }
    public int MutualFundId { get; set; }
    public decimal NAV { get; set; }
    public DateTime Date { get; set; }

    public MutualFund MutualFund { get; set; } = null!;
}

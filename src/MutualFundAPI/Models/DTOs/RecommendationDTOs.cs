namespace MutualFundAPI.Models.DTOs;

public class RecommendationResponseDTO
{
    public int Id { get; set; }
    public string RiskProfile { get; set; } = string.Empty;
    public List<AllocationDTO> Allocations { get; set; } = new();
    public string? AIExplanation { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class AllocationDTO
{
    public string AssetClass { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public string? SuggestedFunds { get; set; }
}

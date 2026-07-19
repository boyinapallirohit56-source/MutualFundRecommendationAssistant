namespace MutualFundAPI.Models.Entities;

public class ChatMessage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}

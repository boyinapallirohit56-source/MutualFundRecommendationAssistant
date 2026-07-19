namespace MutualFundAPI.Models.DTOs;

public class ChatRequestDTO
{
    public string Message { get; set; } = string.Empty;
}

public class ChatResponseDTO
{
    public string Reply { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class ChatHistoryDTO
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

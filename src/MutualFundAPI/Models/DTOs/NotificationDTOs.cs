namespace MutualFundAPI.Models.DTOs;

public class NotificationDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationCountDTO
{
    public int Total { get; set; }
    public int Unread { get; set; }
}

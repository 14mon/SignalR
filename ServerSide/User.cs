namespace SignalR.Models;

public class User
{
    public int Id { get; set; }
    public string? ConnectionId { get; set; }
    public string? UserId { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? DisconnectedAt { get; set; }
}
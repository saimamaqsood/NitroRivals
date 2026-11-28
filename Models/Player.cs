namespace NitroRivals.Models;

public class Player
{
    public int PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int Coins { get; set; }
    public int NitroPoints { get; set; }
    public int? CurrentTierId { get; set; }
    public int? CurrentLevelId { get; set; }
    public DateTime CreatedAt { get; set; }
}
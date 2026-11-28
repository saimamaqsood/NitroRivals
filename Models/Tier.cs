namespace NitroRivals.Models;

public class Tier
{
    public int TierId { get; set; }
    public string TierName { get; set; } = string.Empty;
    public int TierOrder { get; set; }
    public int MinPoints { get; set; }
    public decimal SpeedMultiplier { get; set; } = 1.00m;
}
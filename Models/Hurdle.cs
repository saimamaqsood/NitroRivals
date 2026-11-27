namespace NitroRivals.Models;

public class Hurdle
{
    public int HurdleId { get; set; }
    public string HurdleName { get; set; } = string.Empty;
    public string HurdleType { get; set; } = string.Empty;
    public decimal DamageOrSlowFactor { get; set; }
    public int ScorePenalty { get; set; }
}
namespace NitroRivals.Models;

public class LadderLevel
{
    public int LevelId { get; set; }
    public int LevelNumber { get; set; }
    public string LevelTitle { get; set; } = string.Empty;
    public int CupId { get; set; }
    public int RequiredTierId { get; set; }
    public int TargetScore { get; set; }
    public int RewardCoins { get; set; }
}
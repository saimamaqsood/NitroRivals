namespace NitroRivals.Models;

public class ChampionshipCup
{
    public int CupId { get; set; }
    public string CupName { get; set; } = string.Empty;
    public int EntryFeeCoins { get; set; }
    public int PrizeCoins { get; set; }
    public int RequiredTierOrder { get; set; }
    public bool IsUnlockedByDefault { get; set; }
}
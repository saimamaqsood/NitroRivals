using NitroRivals.Models;

namespace NitroRivals.Drivers;

public class AggressiveRivalDriver : BaseRivalDriver
{
    public AggressiveRivalDriver(Driver profile) : base(profile) { }

    public override decimal CalculateEffectiveSpeed(decimal tierMultiplier)
    {
        // Aggression ki wajah se extra speed boost
        decimal boost = 1 + (Profile.Aggression * 0.15m);
        return Profile.BaseSpeed * tierMultiplier * boost;
    }
}
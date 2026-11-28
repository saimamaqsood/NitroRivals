using NitroRivals.Models;

namespace NitroRivals.Drivers;

public abstract class BaseRivalDriver
{
    public Driver Profile { get; }

    protected BaseRivalDriver(Driver profile)
    {
        Profile = profile;
    }

    // Har AI driver ki speed calculation ka apna tareeqa hoga
    public abstract decimal CalculateEffectiveSpeed(decimal tierMultiplier);
}
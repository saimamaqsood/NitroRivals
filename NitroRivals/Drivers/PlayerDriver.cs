using NitroRivals.Models;

namespace NitroRivals.Drivers;

/// <summary>The human player. The race form feeds keyboard input into it.</summary>
public class PlayerDriver : DriverBase
{
    public PlayerDriver(string name, Vehicle vehicle) : base(name, vehicle) { }

    /// <summary>Set true while the nitro key (Space) is held.</summary>
    public bool NitroPressed { get; set; }

    /// <summary>-1 = left lane, +1 = right lane.</summary>
    public void MoveLane(int delta) => Lane = Lane + delta;

    protected override bool WantsNitro(double dt, RaceContext context) => NitroPressed;
}

using NitroRivals.Models;

namespace NitroRivals.Hurdles;

public class StoppedCarHurdle : Hurdle
{
    public StoppedCarHurdle(double position, int lane) : base(position, lane) { }

    public override string Name => "Stopped car";

    public override HurdleEffect OnHit() => new(0.30, 1.0);
}

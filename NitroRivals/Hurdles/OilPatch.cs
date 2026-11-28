using NitroRivals.Models;

namespace NitroRivals.Hurdles;

public class OilPatch : Hurdle
{
    public OilPatch(double position, int lane) : base(position, lane) { }

    public override string Name => "Oil patch";

    public override HurdleEffect OnHit() => new(0.70, 0.8);
}

using NitroRivals.Models;

namespace NitroRivals.Hurdles;

public class TyreHurdle : Hurdle
{
    public TyreHurdle(double position, int lane) : base(position, lane) { }

    public override string Name => "Tyre barrier";

    public override HurdleEffect OnHit() => new(0.60, 0.4);
}

namespace NitroRivals.Hurdles;

/// <summary>What a hurdle does to a driver: slow down for a short time.</summary>
public readonly record struct HurdleEffect(double SpeedMultiplier, double StunSeconds);

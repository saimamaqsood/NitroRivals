namespace NitroRivals.Models;

/// <summary>Heavy and stable on straights, weakest nitro and cornering.</summary>
public class Truck : Vehicle
{
    public Truck() { }

    public Truck(string modelName, double baseSpeed, double baseHandling, double baseNitro)
        : base(modelName, baseSpeed, baseHandling, baseNitro) { }

    public Truck(Truck other) : base(other) { }

    public override string VehicleClass => "Truck";

    public override double ApplyNitro(double currentSpeed) => currentSpeed * 1.15;

    public override double CalculateCornerSpeed(double currentSpeed) =>
        currentSpeed * Math.Clamp(0.35 + 0.30 * HandlingFactor, 0.30, 0.90);

    public override Vehicle Clone() => new Truck(this);
}

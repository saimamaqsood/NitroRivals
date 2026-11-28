namespace NitroRivals.Models;

/// <summary>Big nitro punch on straights, but loses more speed in corners.</summary>
public class MuscleCar : Vehicle
{
    public MuscleCar() { }

    public MuscleCar(string modelName, double baseSpeed, double baseHandling, double baseNitro)
        : base(modelName, baseSpeed, baseHandling, baseNitro) { }

    public MuscleCar(MuscleCar other) : base(other) { }

    public override string VehicleClass => "Muscle";

    public override double ApplyNitro(double currentSpeed) => currentSpeed * 1.35;

    public override double CalculateCornerSpeed(double currentSpeed) =>
        currentSpeed * Math.Clamp(0.40 + 0.35 * HandlingFactor, 0.30, 0.95);

    public override Vehicle Clone() => new MuscleCar(this);
}

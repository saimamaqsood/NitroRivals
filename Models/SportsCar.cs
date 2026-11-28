namespace NitroRivals.Models;

/// <summary>Fast and agile: best cornering, medium nitro boost.</summary>
public class SportsCar : Vehicle
{
    public SportsCar() { }

    public SportsCar(string modelName, double baseSpeed, double baseHandling, double baseNitro)
        : base(modelName, baseSpeed, baseHandling, baseNitro) { }

    public SportsCar(SportsCar other) : base(other) { }

    public override string VehicleClass => "Sports";

    public override double ApplyNitro(double currentSpeed) => currentSpeed * 1.25;

    public override double CalculateCornerSpeed(double currentSpeed) =>
        currentSpeed * Math.Clamp(0.55 + 0.40 * HandlingFactor, 0.30, 0.97);

    public override Vehicle Clone() => new SportsCar(this);
}

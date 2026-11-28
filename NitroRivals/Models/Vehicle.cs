namespace NitroRivals.Models;

/// <summary>
/// Abstract base for every car. Cannot be instantiated directly (Abstraction).
/// Subclasses decide how nitro and cornering behave (Polymorphism).
/// </summary>
public abstract class Vehicle
{
    public const int MaxUpgradeLevel = 5;

    private string _modelName = "Unknown";
    private double _baseSpeed = 60;
    private double _baseHandling = 60;
    private double _baseNitro = 50;
    private int _engineLevel;
    private int _tireLevel;
    private int _nitroLevel;

    // Default constructor
    protected Vehicle() { }

    // Parameterised constructor
    protected Vehicle(string modelName, double baseSpeed, double baseHandling, double baseNitro)
    {
        ModelName = modelName;
        BaseSpeed = baseSpeed;
        BaseHandling = baseHandling;
        BaseNitro = baseNitro;
    }

    // Copy constructor: used to preview upgrades without touching the real car
    protected Vehicle(Vehicle other)
    {
        ArgumentNullException.ThrowIfNull(other);
        ModelName = other.ModelName;
        BaseSpeed = other.BaseSpeed;
        BaseHandling = other.BaseHandling;
        BaseNitro = other.BaseNitro;
        EngineLevel = other.EngineLevel;
        TireLevel = other.TireLevel;
        NitroLevel = other.NitroLevel;
    }

    public string ModelName
    {
        get => _modelName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Model name cannot be empty.");
            _modelName = value.Trim();
        }
    }

    public double BaseSpeed
    {
        get => _baseSpeed;
        set => _baseSpeed = InRange(value, 1, 200, nameof(BaseSpeed));
    }

    public double BaseHandling
    {
        get => _baseHandling;
        set => _baseHandling = InRange(value, 1, 200, nameof(BaseHandling));
    }

    public double BaseNitro
    {
        get => _baseNitro;
        set => _baseNitro = InRange(value, 1, 200, nameof(BaseNitro));
    }

    public int EngineLevel
    {
        get => _engineLevel;
        set => _engineLevel = (int)InRange(value, 0, MaxUpgradeLevel, nameof(EngineLevel));
    }

    public int TireLevel
    {
        get => _tireLevel;
        set => _tireLevel = (int)InRange(value, 0, MaxUpgradeLevel, nameof(TireLevel));
    }

    public int NitroLevel
    {
        get => _nitroLevel;
        set => _nitroLevel = (int)InRange(value, 0, MaxUpgradeLevel, nameof(NitroLevel));
    }

    /// <summary>"Sports", "Muscle" or "Truck" (matches Cars.VehicleClass in the database).</summary>
    public abstract string VehicleClass { get; }

    /// <summary>Top speed in metres per second, boosted by engine upgrades.</summary>
    public double TopSpeed => BaseSpeed * (1 + 0.05 * EngineLevel) * 0.4;

    /// <summary>How many seconds of nitro the car carries per race.</summary>
    public double NitroSeconds => BaseNitro / 10.0 * (1 + 0.1 * NitroLevel);

    /// <summary>Handling after tyre upgrades (about 0.3 to 1.2).</summary>
    protected double HandlingFactor => BaseHandling / 100.0 * (1 + 0.05 * TireLevel);

    /// <summary>Speed while nitro is burning.</summary>
    public abstract double ApplyNitro(double currentSpeed);

    /// <summary>Maximum speed the car can hold through a corner.</summary>
    public abstract double CalculateCornerSpeed(double currentSpeed);

    /// <summary>Copy of this car (uses the copy constructor of the subclass).</summary>
    public abstract Vehicle Clone();

    public override string ToString() =>
        $"{ModelName} ({VehicleClass}) E{EngineLevel}/T{TireLevel}/N{NitroLevel}";

    private static double InRange(double value, double min, double max, string name)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(name, $"{name} must be between {min} and {max}.");
        return value;
    }
}

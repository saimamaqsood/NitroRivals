using NitroRivals.Models;

namespace NitroRivals.Drivers;

/// <summary>
/// Shared physics for every driver. Subclasses only decide *what they want*
/// (use nitro? change lane?) - the movement rules live here (Inheritance).
/// </summary>
public abstract class DriverBase : IDriver
{
    private const double Acceleration = 18.0;   // m/s^2
    private const double Braking = 45.0;        // m/s^2

    private readonly HashSet<Hurdle> _hitHurdles = new();
    private double _elapsed;
    private double _stunTimer;
    private double _stunMultiplier = 1.0;
    private int _lane = 1;

    protected DriverBase(string name, Vehicle vehicle)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        NitroRemaining = vehicle.NitroSeconds;
    }

    public string Name { get; }
    public Vehicle Vehicle { get; }
    public double Distance { get; private set; }
    public double Speed { get; private set; }
    public bool IsNitroActive { get; private set; }
    public double NitroRemaining { get; private set; }
    public int HurdlesHit { get; private set; }
    public bool IsFinished { get; private set; }
    public double FinishTime { get; private set; }

    public int Lane
    {
        get => _lane;
        protected set => _lane = Math.Clamp(value, 0, Track.LaneCount - 1);
    }

    /// <summary>1.0 = full vehicle speed. AI drivers go below this on low difficulty.</summary>
    protected virtual double SpeedFactor => 1.0;

    /// <summary>1.0 = perfect cornering.</summary>
    protected virtual double CornerSkill => 1.0;

    protected abstract bool WantsNitro(double dt, RaceContext context);

    protected virtual void UpdateLane(double dt, RaceContext context) { }

    public void Update(double dt, RaceContext context)
    {
        if (IsFinished || dt <= 0) return;
        _elapsed += dt;

        UpdateLane(dt, context);

        // 1. Target speed from the car's stats
        double target = Vehicle.TopSpeed * SpeedFactor;

        // 2. Nitro
        bool nitro = NitroRemaining > 0 && WantsNitro(dt, context);
        IsNitroActive = nitro;
        if (nitro)
        {
            target = Vehicle.ApplyNitro(target);
            NitroRemaining = Math.Max(0, NitroRemaining - dt);
        }

        // 3. Corners slow the car down
        if (context.Track.IsInCorner(Distance))
            target = Math.Min(target, Vehicle.CalculateCornerSpeed(target) * CornerSkill);

        // 4. Recent hurdle hit slows the car
        if (_stunTimer > 0)
        {
            target *= _stunMultiplier;
            _stunTimer -= dt;
        }

        // 5. Accelerate or brake towards the target
        Speed = Speed < target
            ? Math.Min(target, Speed + Acceleration * dt)
            : Math.Max(target, Speed - Braking * dt);

        double before = Distance;
        Distance += Speed * dt;

        CheckHurdles(before, context);

        // 6. Finish line
        double length = context.Track.LengthMeters;
        if (Distance >= length)
        {
            double overshoot = Distance - length;
            FinishTime = _elapsed - (Speed > 0 ? overshoot / Speed : 0);
            Distance = length;
            IsFinished = true;
            IsNitroActive = false;
        }
    }

    private void CheckHurdles(double before, RaceContext context)
    {
        foreach (Hurdle h in context.Hurdles)
        {
            if (h.Position > Distance) break;                       // hurdles are sorted
            if (h.Lane != Lane || _hitHurdles.Contains(h)) continue;
            if (before >= h.Position + Hurdle.Length) continue;     // already passed

            var effect = h.OnHit();                                 // polymorphic call
            _stunMultiplier = effect.SpeedMultiplier;
            _stunTimer = effect.StunSeconds;
            Speed *= effect.SpeedMultiplier;
            HurdlesHit++;
            _hitHurdles.Add(h);
        }
    }
}

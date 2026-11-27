using NitroRivals.Models;

namespace NitroRivals.Drivers;

/// <summary>
/// Computer opponent. Difficulty 1-10 controls speed, cornering, nitro use
/// and how quickly it reacts to hurdles.
/// </summary>
public class AiDriver : DriverBase
{
    private readonly Random _rng;
    private double _nitroBurst;          // seconds of nitro left in the current burst
    private double _reaction = -1;       // countdown before swerving, -1 = no threat

    public AiDriver(string name, Vehicle vehicle, int difficulty, Random? rng = null)
        : base(name, vehicle)
    {
        if (difficulty < 1 || difficulty > 10)
            throw new ArgumentOutOfRangeException(nameof(difficulty), "Difficulty must be 1 to 10.");

        Difficulty = difficulty;
        _rng = rng ?? new Random();
        Lane = _rng.Next(Track.LaneCount);
    }

    public int Difficulty { get; }

    protected override double SpeedFactor => 0.78 + 0.022 * Difficulty;    // 0.80 .. 1.00
    protected override double CornerSkill => 0.85 + 0.015 * Difficulty;    // 0.86 .. 1.00

    protected override bool WantsNitro(double dt, RaceContext context)
    {
        if (_nitroBurst > 0)
        {
            _nitroBurst -= dt;
            return true;
        }

        bool onStraight = !context.Track.IsInCorner(Distance);
        if (onStraight && _rng.NextDouble() < dt * (0.15 + 0.03 * Difficulty))
        {
            _nitroBurst = 1.0 + _rng.NextDouble() * 1.5;
            return true;
        }
        return false;
    }

    protected override void UpdateLane(double dt, RaceContext context)
    {
        double lookAhead = 40 + 6 * Difficulty;     // 46 .. 100 metres

        if (FindThreat(context, Lane, lookAhead) == null)
        {
            _reaction = -1;
            return;
        }

        // Higher difficulty reacts faster
        if (_reaction < 0) _reaction = (11 - Difficulty) * 0.06;
        _reaction -= dt;
        if (_reaction > 0) return;

        var candidates = Enumerable.Range(0, Track.LaneCount)
            .Where(l => l != Lane)
            .OrderBy(l => Math.Abs(l - Lane))
            .ToList();

        foreach (int lane in candidates)
        {
            if (FindThreat(context, lane, lookAhead) == null)
            {
                Lane = lane;
                break;
            }
        }
        _reaction = -1;
    }

    private Hurdle? FindThreat(RaceContext context, int lane, double lookAhead) =>
        context.Hurdles.FirstOrDefault(h =>
            h.Lane == lane &&
            h.Position >= Distance - 1 &&
            h.Position - Distance <= lookAhead);
}

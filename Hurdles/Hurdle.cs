using NitroRivals.Hurdles;

namespace NitroRivals.Models;

/// <summary>Abstract road obstacle. Each subclass defines what happens on a hit.</summary>
public abstract class Hurdle
{
    public const double Length = 8;   // metres of road the obstacle covers

    private double _position;
    private int _lane;

    protected Hurdle(double position, int lane)
    {
        Position = position;
        Lane = lane;
    }

    /// <summary>Distance from the start line in metres.</summary>
    public double Position
    {
        get => _position;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(Position));
            _position = value;
        }
    }

    public int Lane
    {
        get => _lane;
        set
        {
            if (value < 0 || value >= Track.LaneCount)
                throw new ArgumentOutOfRangeException(nameof(Lane));
            _lane = value;
        }
    }

    public abstract string Name { get; }

    /// <summary>Effect applied to a driver who hits this hurdle.</summary>
    public abstract HurdleEffect OnHit();
}

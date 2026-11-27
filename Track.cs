namespace NitroRivals.Models;

/// <summary>A race track: a straight distance with corner zones spread along it.</summary>
public class Track
{
    public const int LaneCount = 3;
    public const double CornerLength = 80;   // metres

    private readonly List<double> _corners = new();

    public Track(string name, double lengthMeters, int cornerCount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Track name cannot be empty.");
        if (lengthMeters <= 0)
            throw new ArgumentOutOfRangeException(nameof(lengthMeters));
        if (cornerCount < 0)
            throw new ArgumentOutOfRangeException(nameof(cornerCount));

        Name = name.Trim();
        LengthMeters = lengthMeters;
        CornerCount = cornerCount;

        // Spread corners evenly between start and finish
        for (int i = 1; i <= cornerCount; i++)
            _corners.Add(lengthMeters * i / (cornerCount + 1));
    }

    public string Name { get; }
    public double LengthMeters { get; }
    public int CornerCount { get; }
    public IReadOnlyList<double> CornerPositions => _corners;

    public bool IsInCorner(double distance)
    {
        foreach (double start in _corners)
            if (distance >= start && distance < start + CornerLength)
                return true;
        return false;
    }
}

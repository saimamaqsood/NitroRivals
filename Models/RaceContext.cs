namespace NitroRivals.Models;

/// <summary>Everything a driver needs to know about the race (composition).</summary>
public class RaceContext
{
    public RaceContext(Track track, IEnumerable<Hurdle> hurdles)
    {
        Track = track ?? throw new ArgumentNullException(nameof(track));
        Hurdles = (hurdles ?? throw new ArgumentNullException(nameof(hurdles)))
            .OrderBy(h => h.Position)
            .ToList();
    }

    public Track Track { get; }
    public IReadOnlyList<Hurdle> Hurdles { get; }
}

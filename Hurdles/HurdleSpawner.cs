using NitroRivals.Hurdles;
using NitroRivals.Models;

namespace NitroRivals.Services;

/// <summary>Places hurdles along a track. Higher levels add more variety.</summary>
public static class HurdleSpawner
{
    public static List<Hurdle> Create(Track track, int hurdleCount, int levelNumber, Random? rng = null)
    {
        rng ??= new Random();
        var hurdles = new List<Hurdle>();
        if (hurdleCount <= 0) return hurdles;

        double start = 200;
        double end = track.LengthMeters - 200;
        double slot = (end - start) / hurdleCount;

        for (int i = 0; i < hurdleCount; i++)
        {
            double position = start + slot * i + rng.NextDouble() * slot * 0.6;
            int lane = rng.Next(Track.LaneCount);
            hurdles.Add(CreateRandom(levelNumber, position, lane, rng));
        }
        return hurdles;
    }

    private static Hurdle CreateRandom(int level, double position, int lane, Random rng)
    {
        // Levels 1-3: tyres only. 4-6: + oil. 7-10: + stopped cars.
        int types = level <= 3 ? 1 : level <= 6 ? 2 : 3;

        return rng.Next(types) switch
        {
            0 => (Hurdle)new TyreHurdle(position, lane),
            1 => new OilPatch(position, lane),
            _ => new StoppedCarHurdle(position, lane)
        };
    }
}

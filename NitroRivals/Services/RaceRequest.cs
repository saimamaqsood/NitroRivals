using NitroRivals.Drivers;
using NitroRivals.Hurdles;
using NitroRivals.Models;

namespace NitroRivals.Services;

public static class RaceModes
{
    public const string Quick = "QuickRace";
    public const string QuickRace = "QuickRace";
    public const string TimeTrial = "TimeTrial";
    public const string Level = "Level";
    public const string Cup = "Cup";
}

public class RaceRequest
{
    public string Mode { get; set; } = RaceModes.QuickRace;
    public string Title { get; set; } = "Nitro Rivals Race";
    public int TrackId { get; set; }
    public string TrackName { get; set; } = "City Circuit";
    public double LengthMeters { get; set; } = 1500.0;
    public int CornerCount { get; set; } = 6;
    public int HurdleCount { get; set; } = 5;
    public int AiDifficulty { get; set; } = 3;
    public int LevelNumber { get; set; } = 1;
    public int? LevelId { get; set; }
    public int? CupEntryId { get; set; }
    public int? RoundNumber { get; set; }
}

public static class RaceSetup
{
    public static RaceEngine Build(RaceRequest request, out int carId)
    {
        carId = 1;

        string trackName = string.IsNullOrWhiteSpace(request.TrackName) ? "City Circuit" : request.TrackName;
        double lengthMeters = request.LengthMeters > 0 ? request.LengthMeters : 1500.0;
        int corners = Math.Max(0, request.CornerCount);

        var track = new Track(trackName, lengthMeters, corners);
        var hurdles = new List<Hurdle>();
        var context = new RaceContext(track, hurdles);

        var drivers = new List<IDriver>();
        Vehicle playerCar = VehicleFactory.Create("Sports", "Apex GT", 210, 80, 60);
        var player = new PlayerDriver("Player", playerCar);
        drivers.Add(player);

        if (request.Mode != RaceModes.TimeTrial)
        {
            int difficulty = Math.Clamp(request.AiDifficulty, 1, 10);
            for (int i = 1; i <= 3; i++)
            {
                Vehicle aiCar = VehicleFactory.Create("Muscle", $"Rival {i}", 195, 75, 50);
                drivers.Add(new AiDriver($"Rival {i}", aiCar, difficulty));
            }
        }

        return new RaceEngine(request, context, player, drivers, carId);
    }
}
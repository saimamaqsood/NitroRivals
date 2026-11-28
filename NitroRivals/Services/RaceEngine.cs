using NitroRivals.Drivers;
using NitroRivals.Models;

namespace NitroRivals.Services;

public class RaceEngine
{
    private readonly List<IDriver> _drivers;

    public RaceRequest Request { get; }
    public RaceContext Context { get; }
    public PlayerDriver Player { get; }
    public int PlayerCarId { get; }
    public IReadOnlyList<IDriver> Drivers => _drivers;
    public double Elapsed { get; private set; }
    public bool IsFinished => Player.IsFinished;

    public RaceEngine(
        RaceRequest request,
        RaceContext context,
        PlayerDriver player,
        List<IDriver> drivers,
        int playerCarId)
    {
        Request = request;
        Context = context;
        Player = player;
        _drivers = drivers;
        PlayerCarId = playerCarId;
    }

    public void Tick(double dt)
    {
        if (IsFinished) return;

        Elapsed += dt;
        foreach (var driver in _drivers)
        {
            driver.Update(dt, Context);
        }
    }

    public int GetPosition(IDriver target)
    {
        var ordered = _drivers
            .OrderByDescending(d => d.Distance)
            .ThenBy(d => d.FinishTime > 0 ? d.FinishTime : double.MaxValue)
            .ToList();

        int index = ordered.IndexOf(target);
        return index >= 0 ? index + 1 : 1;
    }
}
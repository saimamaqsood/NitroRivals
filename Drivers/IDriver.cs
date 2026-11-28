using NitroRivals.Models;

namespace NitroRivals.Drivers;

/// <summary>Anything that races: the human player or an AI opponent.</summary>
public interface IDriver
{
    string Name { get; }
    Vehicle Vehicle { get; }

    double Distance { get; }        // metres from the start line
    double Speed { get; }           // metres per second
    int Lane { get; }               // 0 .. Track.LaneCount - 1
    bool IsNitroActive { get; }
    double NitroRemaining { get; }  // seconds
    int HurdlesHit { get; }

    bool IsFinished { get; }
    double FinishTime { get; }      // seconds, valid once IsFinished

    /// <summary>Advance the driver by dt seconds.</summary>
    void Update(double dt, RaceContext context);
}

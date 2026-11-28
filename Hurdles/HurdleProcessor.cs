using NitroRivals.Models;

namespace NitroRivals.Hurdles;

public class HurdleProcessor
{
    public (decimal NewSpeed, int NewScore) ApplyHurdleImpact(Hurdle hurdle, decimal currentSpeed, int currentScore)
    {
        decimal reducedSpeed = currentSpeed * (1 - hurdle.DamageOrSlowFactor);
        int updatedScore = Math.Max(0, currentScore - hurdle.ScorePenalty);
        return (Math.Round(reducedSpeed, 2), updatedScore);
    }
}
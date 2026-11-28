using NitroRivals.Models;

namespace NitroRivals.Hurdles;

public class HurdleProcessor
{
    public (decimal NewSpeed, int NewScore) ApplyHurdleImpact(Hurdle hurdle, decimal currentSpeed, int currentScore)
    {
        HurdleEffect effect = hurdle.OnHit();
        decimal factor = Convert.ToDecimal(effect.SpeedMultiplier);
        decimal reducedSpeed = currentSpeed * factor;
        int penalty = effect.StunSeconds > 0 ? (int)Math.Round(effect.StunSeconds * 10) : 10;
        int updatedScore = Math.Max(0, currentScore - penalty);
        return (Math.Round(reducedSpeed, 2), updatedScore);
    }
}
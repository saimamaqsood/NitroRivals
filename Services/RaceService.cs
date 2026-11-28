using NitroRivals.Exceptions;
using NitroRivals.Models;
using NitroRivals.Repositories;

namespace NitroRivals.Services;

public class RaceService
{
    private readonly GameRepository _repository;

    public RaceService(GameRepository repository)
    {
        _repository = repository;
    }

    public List<LadderLevel> LoadLadderLevels()
    {
        var levels = _repository.GetAllLevels();
        if (levels.Count == 0)
        {
            throw new GameException("Database mein koi ladder level nahi mila. Seed script check karein.");
        }
        return levels;
    }

    public bool IsLevelPassed(LadderLevel level, int achievedScore)
    {
        return achievedScore >= level.TargetScore;
    }
}
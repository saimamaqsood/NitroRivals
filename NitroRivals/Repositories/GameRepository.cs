using Microsoft.Data.SqlClient;
using NitroRivals.Models;

namespace NitroRivals.Repositories;

public class GameRepository
{
    public List<LadderLevel> GetAllLevels()
    {
        var levels = new List<LadderLevel>();

        using var conn = DbHelper.GetConnection();
        conn.Open();

        const string query = @"
            SELECT LevelId, LevelNumber, LevelTitle, CupId, RequiredTierId, TargetScore, RewardCoins
            FROM LadderLevels
            ORDER BY LevelNumber;";

        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            levels.Add(new LadderLevel
            {
                LevelId = reader.GetInt32(0),
                LevelNumber = reader.GetInt32(1),
                LevelTitle = reader.GetString(2),
                CupId = reader.GetInt32(3),
                RequiredTierId = reader.GetInt32(4),
                TargetScore = reader.GetInt32(5),
                RewardCoins = reader.GetInt32(6)
            });
        }

        return levels;
    }
}
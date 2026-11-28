using System.Data;
using Microsoft.Data.SqlClient;
using NitroRivals.Services;

namespace NitroRivals.Repositories;

public class RaceRepository
{
    public DataTable GetLevels(int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM Levels", conn);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataTable GetTracks()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM Tracks", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataRow SaveRaceResult(RaceRequest request, int playerId, int playerCarId, int position, double time)
    {
        int coinsEarned = position == 1 ? 150 : position == 2 ? 100 : 60;
        int pointsEarned = position == 1 ? 50 : position == 2 ? 30 : 15;
        int newCoins = Session.Coins + coinsEarned;

        var dt = new DataTable();
        dt.Columns.Add("NewCoins", typeof(int));
        dt.Columns.Add("CoinsEarned", typeof(int));
        dt.Columns.Add("PointsEarned", typeof(int));
        dt.Columns.Add("RankTier", typeof(string));
        dt.Columns.Add("UnlockedNextLevel", typeof(bool));

        dt.Rows.Add(newCoins, coinsEarned, pointsEarned, "Bronze", position == 1);
        return dt.Rows[0];
    }
    public DataTable GetLeaderboard()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT TOP 20 * FROM Players ORDER BY Coins DESC", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }
}
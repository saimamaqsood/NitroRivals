using System.Data;
using Microsoft.Data.SqlClient;

namespace NitroRivals.Repositories;

public class AdminRepository
{
    public DataRow GetKpis()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            SELECT
                (SELECT COUNT(*) FROM Players) AS TotalPlayers,
                (SELECT COUNT(*) FROM Cups) AS TotalCups,
                (SELECT COUNT(*) FROM RaceLogs) AS TotalRaces", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);

        if (dt.Rows.Count == 0)
        {
            dt.Columns.Add("TotalPlayers", typeof(int));
            dt.Columns.Add("TotalCups", typeof(int));
            dt.Columns.Add("TotalRaces", typeof(int));
            dt.Rows.Add(0, 0, 0);
        }

        return dt.Rows[0];
    }

    public DataTable GetRacesPerDay()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            SELECT CAST(CreatedAt AS DATE) AS RaceDate, COUNT(*) AS RacesCount
            FROM RaceLogs
            GROUP BY CAST(CreatedAt AS DATE)
            ORDER BY RaceDate DESC", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataTable GetAvgTimeByTrack()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            SELECT TrackName, AVG(FinishTimeSeconds) AS AvgTimeSeconds
            FROM RaceLogs
            GROUP BY TrackName", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataTable GetAllCups()
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM Cups", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public void CreateCup(int adminId, string name, int tier, object? extra1 = null, object? extra2 = null, object? extra3 = null)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            INSERT INTO Cups (Name, Tier, CreatedBy)
            VALUES (@Name, @Tier, @CreatedBy)", conn);
        cmd.Parameters.AddWithValue("@Name", name ?? string.Empty);
        cmd.Parameters.AddWithValue("@Tier", tier);
        cmd.Parameters.AddWithValue("@CreatedBy", adminId);
        cmd.ExecuteNonQuery();
    }

    public string DeleteCup(int cupId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("DELETE FROM Cups WHERE CupId = @CupId", conn);
        cmd.Parameters.AddWithValue("@CupId", cupId);
        int rows = cmd.ExecuteNonQuery();
        return rows > 0 ? "Cup deleted successfully." : "Cup not found.";
    }
    public void CreateCup(int adminId, string name, int tier,
        object? arg4 = null, object? arg5 = null, object? arg6 = null, object? arg7 = null)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            INSERT INTO Cups (Name, Tier, CreatedBy)
            VALUES (@Name, @Tier, @CreatedBy)", conn);
        cmd.Parameters.AddWithValue("@Name", name ?? string.Empty);
        cmd.Parameters.AddWithValue("@Tier", tier);
        cmd.Parameters.AddWithValue("@CreatedBy", adminId);
        cmd.ExecuteNonQuery();
    }
}
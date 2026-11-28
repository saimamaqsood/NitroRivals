using System.Data;
using Microsoft.Data.SqlClient;

namespace NitroRivals.Repositories;

public class CupRepository
{
    public DataTable GetCups(int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM Cups", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public void EnterCup(int playerId, int cupId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            INSERT INTO CupEntries (CupId, PlayerId)
            VALUES (@CupId, @PlayerId)", conn);
        cmd.Parameters.AddWithValue("@CupId", cupId);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        cmd.ExecuteNonQuery();
    }

    public DataRow GetCupEntry(int entryId, int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM CupEntries WHERE EntryId = @EntryId AND PlayerId = @PlayerId", conn);
        cmd.Parameters.AddWithValue("@EntryId", entryId);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);

        if (dt.Rows.Count == 0)
        {
            dt.Columns.Add("EntryId", typeof(int));
            dt.Columns.Add("PlayerId", typeof(int));
            dt.Rows.Add(entryId, playerId);
        }

        return dt.Rows[0];
    }

    public DataRow FinishCup(int entryId, int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            UPDATE CupEntries SET IsCompleted = 1 WHERE EntryId = @EntryId AND PlayerId = @PlayerId;
            SELECT * FROM CupEntries WHERE EntryId = @EntryId AND PlayerId = @PlayerId;", conn);
        cmd.Parameters.AddWithValue("@EntryId", entryId);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);

        if (dt.Rows.Count == 0)
        {
            dt.Columns.Add("Status", typeof(string));
            dt.Rows.Add("Completed");
        }

        return dt.Rows[0];
    }
}
using System.Data;
using Microsoft.Data.SqlClient;

namespace NitroRivals.Repositories;

public class GarageRepository
{
    public DataTable GetGarage(int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM PlayerCars WHERE PlayerId = @PlayerId", conn);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataTable GetShopCars(int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT * FROM Cars", conn);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public int GetCoins(int playerId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("SELECT COALESCE(Coins, 0) FROM Players WHERE PlayerId = @PlayerId", conn);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        object? result = cmd.ExecuteScalar();
        return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
    }

    public void BuyCar(int playerId, int carId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(@"
            INSERT INTO PlayerCars (PlayerId, CarId, IsEquipped)
            VALUES (@PlayerId, @CarId, 0)", conn);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        cmd.Parameters.AddWithValue("@CarId", carId);
        cmd.ExecuteNonQuery();
    }

    public void EquipCar(int playerId, int playerCarId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var resetCmd = new SqlCommand("UPDATE PlayerCars SET IsEquipped = 0 WHERE PlayerId = @PlayerId", conn);
        resetCmd.Parameters.AddWithValue("@PlayerId", playerId);
        resetCmd.ExecuteNonQuery();

        using var equipCmd = new SqlCommand("UPDATE PlayerCars SET IsEquipped = 1 WHERE PlayerCarId = @Id AND PlayerId = @PlayerId", conn);
        equipCmd.Parameters.AddWithValue("@Id", playerCarId);
        equipCmd.Parameters.AddWithValue("@PlayerId", playerId);
        equipCmd.ExecuteNonQuery();
    }

    public void UpgradeCar(int playerId, int playerCarId, string part)
    {
        string column = part switch
        {
            "Engine" => "EngineLevel",
            "Tire" => "TireLevel",
            "Nitro" => "NitroLevel",
            _ => "EngineLevel"
        };

        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand($"UPDATE PlayerCars SET {column} = COALESCE({column}, 1) + 1 WHERE PlayerCarId = @Id AND PlayerId = @PlayerId", conn);
        cmd.Parameters.AddWithValue("@Id", playerCarId);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        cmd.ExecuteNonQuery();
    }

    public void SellCar(int playerId, int playerCarId)
    {
        using var conn = DbHelper.GetConnection();
        conn.Open();
        using var cmd = new SqlCommand("DELETE FROM PlayerCars WHERE PlayerCarId = @Id AND PlayerId = @PlayerId", conn);
        cmd.Parameters.AddWithValue("@Id", playerCarId);
        cmd.Parameters.AddWithValue("@PlayerId", playerId);
        cmd.ExecuteNonQuery();
    }
}
using System.Data;
using Microsoft.Data.SqlClient;

namespace NitroRivals.Repositories;

public static class DbHelper
{
    // Apne SSMS server name ke mutabiq Server check kar lein (maslan localhost ya DESKTOP-7GR5BSQ\SAMEER...)
    private const string ConnectionString =
        "Server=localhost;Database=NitroRivalsDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    public static SqlParameter Param(string name, object? value)
    {
        return new SqlParameter(name, value ?? DBNull.Value);
    }

    public static object? ExecuteScalar(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    public static DataTable ExecuteTable(string sql, params SqlParameter[] parameters)
    {
        using var conn = GetConnection();
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        if (parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }
    public static SqlParameter Param(string name, SqlDbType dbType, object? value)
    {
        var p = new SqlParameter(name, dbType)
        {
            Value = value ?? DBNull.Value
        };
        return p;
    }

    public static SqlParameter Param(string name, SqlDbType dbType, object? value, int size)
    {
        var p = new SqlParameter(name, dbType, size)
        {
            Value = value ?? DBNull.Value
        };
        return p;
    }
}
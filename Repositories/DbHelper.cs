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
}
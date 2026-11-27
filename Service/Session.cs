namespace NitroRivals.Services;

/// <summary>Holds who is currently logged in.</summary>
public static class Session
{
    public static int Id { get; private set; }
    public static string Username { get; private set; } = "";
    public static bool IsAdmin { get; private set; }
    public static int Coins { get; set; }
    public static bool IsLoggedIn => Id != 0;

    public static void StartPlayer(int id, string username, int coins)
    { Id = id; Username = username; Coins = coins; IsAdmin = false; }

    public static void StartAdmin(int id, string username)
    { Id = id; Username = username; Coins = 0; IsAdmin = true; }

    public static void Clear()
    { Id = 0; Username = ""; Coins = 0; IsAdmin = false; }
}

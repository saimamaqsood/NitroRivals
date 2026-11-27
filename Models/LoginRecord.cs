namespace NitroRivals.Models;

/// <summary>Row returned by sp_GetPlayerLogin / sp_GetAdminLogin.</summary>
public record LoginRecord(
    int Id,
    string Username,
    string Email,
    byte[] Hash,
    byte[] Salt,
    bool IsVerified,
    int Coins);

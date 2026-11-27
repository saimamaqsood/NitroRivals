using System.Net.Mail;
using NitroRivals.Repositories;

namespace NitroRivals.Services;

public class AuthService
{
    private readonly PlayerRepository _repo = new();

    /// <summary>Validates input, registers the player, emails the OTP. Returns the new PlayerId.</summary>
    public int Register(string username, string email, string password, string confirm)
    {
        username = username.Trim();
        email = email.Trim();

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException("Username must be 3 to 50 characters.");
        if (!IsValidEmail(email))
            throw new ArgumentException("Please enter a valid email address.");
        if (password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");
        if (password != confirm)
            throw new ArgumentException("Passwords do not match.");

        byte[] hash = PasswordHasher.HashPassword(password, out byte[] salt);
        string otp = PasswordHasher.GenerateOtp();

        int playerId = _repo.Register(username, email, hash, salt, PasswordHasher.HashOtp(otp, email));
        EmailService.SendOtp(email, otp);
        return playerId;
    }

    public void VerifyOtp(int playerId, string email, string otp)
    {
        if (otp.Length != 6 || !otp.All(char.IsDigit))
            throw new ArgumentException("The code must be exactly 6 digits.");
        _repo.VerifyOtp(playerId, PasswordHasher.HashOtp(otp, email));
    }

    public void ResendOtp(int playerId, string email)
    {
        string otp = PasswordHasher.GenerateOtp();
        _repo.ResendOtp(playerId, PasswordHasher.HashOtp(otp, email));
        EmailService.SendOtp(email, otp);
    }

    public LoginResult Login(string username, string password, out int playerId, out string email)
    {
        playerId = 0;
        email = "";
        username = username.Trim();

        var player = _repo.GetPlayerLogin(username);
        if (player != null)
        {
            if (!PasswordHasher.Verify(password, player.Hash, player.Salt))
                return LoginResult.InvalidCredentials;
            if (!player.IsVerified)
            {
                playerId = player.Id;
                email = player.Email;
                return LoginResult.NotVerified;
            }
            Session.StartPlayer(player.Id, player.Username, player.Coins);
            return LoginResult.Success;
        }

        var admin = _repo.GetAdminLogin(username);
        if (admin != null && PasswordHasher.Verify(password, admin.Hash, admin.Salt))
        {
            Session.StartAdmin(admin.Id, admin.Username);
            return LoginResult.Success;
        }
        return LoginResult.InvalidCredentials;
    }

    /// <summary>Gives the seeded 'admin' account a real password (Admin@123) on first run.</summary>
    public void EnsureAdminSeeded()
    {
        byte[] hash = PasswordHasher.HashPassword("Admin@123", out byte[] salt);
        _repo.SeedAdminPassword(hash, salt);
    }

    private static bool IsValidEmail(string email)
    {
        try { return new MailAddress(email).Address == email; }
        catch { return false; }
    }
}

using System.Net;
using System.Net.Mail;

namespace NitroRivals.Services;

public static class EmailService
{
    public static void SendOtp(string email, string otp)
    {
        if (GameSettings.DevMode)
        {
            MessageBox.Show($"[DEV MODE] Verification code for {email}:\n\n{otp}",
                "Nitro Rivals", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var message = new MailMessage(GameSettings.SmtpFrom, email,
            "Nitro Rivals verification code",
            $"Your Nitro Rivals verification code is {otp}. It expires in 5 minutes.");
        using var client = new SmtpClient(GameSettings.SmtpHost, GameSettings.SmtpPort)
        {
            Credentials = new NetworkCredential(GameSettings.SmtpUser, GameSettings.SmtpPassword),
            EnableSsl = true
        };
        client.Send(message);
    }
}

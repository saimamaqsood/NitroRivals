namespace NitroRivals.Services;

public static class GameSettings
{
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=NitroRivals;Integrated Security=true;TrustServerCertificate=true";

    /// <summary>true = OTP is shown in a MessageBox instead of being emailed.</summary>
    public static bool DevMode { get; set; } = true;

    // SMTP (only used when DevMode = false). Use a Gmail App Password.
    public static string SmtpHost { get; set; } = "smtp.gmail.com";
    public static int    SmtpPort { get; set; } = 587;
    public static string SmtpUser { get; set; } = "yourgame@gmail.com";
    public static string SmtpPassword { get; set; } = "your-app-password";
    public static string SmtpFrom { get; set; } = "yourgame@gmail.com";
}

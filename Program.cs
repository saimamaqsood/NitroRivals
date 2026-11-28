using NitroRivals.Forms;
using NitroRivals.Services;

namespace NitroRivals;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            new AuthService().EnsureAdminSeeded();   // sets admin password on first run
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Could not connect to the database.\n\n" +
                "Did you run standalone.sql and 02_auth_procedures.sql?\n\n" + ex.Message,
                "Nitro Rivals", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new LoginForm());
    }
}

using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class MainMenuForm : Form
{
    private static readonly string[] PlayerScreens =
        { "Garage", "Shop", "Levels", "Quick Race", "Time Trial", "Cups", "Leaderboard" };
    private static readonly string[] AdminScreens =
        { "Admin Dashboard", "Cup Management" };

    private readonly Label? _lblCoins;

    public MainMenuForm()
    {
        string[] screens = Session.IsAdmin ? AdminScreens : PlayerScreens;
        int rows = (screens.Length + 1) / 2;
        int height = 190 + rows * 60 + 70;

        FormUi.Setup(this, "Nitro Rivals - Main Menu", 460, height);

        FormUi.AddLabel(this, "MAIN MENU", 30, 20, 20f, true);
        FormUi.AddLabel(this,
            Session.IsAdmin ? $"Administrator: {Session.Username}" : $"Welcome, {Session.Username}",
            32, 70, 11f);
        if (!Session.IsAdmin)
            _lblCoins = FormUi.AddLabel(this, $"Coins: {Session.Coins}", 32, 98, 11f, bold: true);

        for (int i = 0; i < screens.Length; i++)
        {
            string name = screens[i];
            int x = 30 + (i % 2) * 205;
            int y = 150 + (i / 2) * 60;
            var btn = FormUi.AddButton(this, name, x, y, 195, 48);
            btn.Click += (s, e) => Open(name);
        }

        var logout = FormUi.AddButton(this, "Logout", 30, height - 60, 400, 40, primary: false);
        logout.Click += (s, e) => Close();
    }

    private void Open(string screen)
    {
        try
        {
            using Form form = screen switch
            {
                "Garage"          => new GarageForm(),
                "Shop"            => new ShopForm(),
                "Levels"          => new LevelsForm(),
                "Quick Race"      => new RaceSetupForm(RaceModes.Quick),
                "Time Trial"      => new RaceSetupForm(RaceModes.TimeTrial),
                "Cups"            => new CupsForm(),
                "Leaderboard"     => new LeaderboardForm(),
                "Admin Dashboard" => new AdminDashboardForm(),
                "Cup Management"  => new CupManagementForm(),
                _ => throw new InvalidOperationException("Unknown screen.")
            };
            form.ShowDialog(this);
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not open " + screen + ": " + ex.Message);
        }
        RefreshCoins();
    }

    private void RefreshCoins()
    {
        if (_lblCoins == null) return;
        try
        {
            Session.Coins = new GarageRepository().GetCoins(Session.Id);
            _lblCoins.Text = $"Coins: {Session.Coins}";
        }
        catch { /* leave the old value */ }
    }
}

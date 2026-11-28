using NitroRivals.Repositories;

namespace NitroRivals.Forms;

public class LeaderboardForm : Form
{
    public LeaderboardForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Leaderboard", 700, 540);

        FormUi.AddLabel(this, "LEADERBOARD", 20, 15, 20f, true);
        FormUi.AddLabel(this, "Top 20 verified players by rank points.", 22, 52, 9.5f, muted: true);

        var grid = FormUi.AddGrid(this, 20, 85, 660, 380);
        var btnClose = FormUi.AddButton(this, "Back", 20, 480, 150, primary: false);
        btnClose.Click += (s, e) => Close();

        try
        {
            grid.DataSource = new RaceRepository().GetLeaderboard();
            FormUi.Header(grid, "Position", "#");
            FormUi.Header(grid, "RankTier", "Tier");
            FormUi.Header(grid, "RankPoints", "Rank points");
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load the leaderboard: " + ex.Message);
        }
    }
}

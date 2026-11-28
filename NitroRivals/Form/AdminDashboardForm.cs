using System.Data;
using NitroRivals.Repositories;

namespace NitroRivals.Forms;

public class AdminDashboardForm : Form
{
    private readonly AdminRepository _repo = new();
    private readonly Dictionary<string, Label> _kpis = new();
    private readonly DataGridView _gridDays;
    private readonly DataGridView _gridTracks;

    public AdminDashboardForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Admin Dashboard", 900, 640);

        FormUi.AddLabel(this, "ADMIN DASHBOARD", 20, 15, 20f, true);
        FormUi.AddLabel(this, "Live figures from the SQL views.", 22, 52, 9.5f, muted: true);

        AddKpi("RacesToday", "Races today", 20, 90);
        AddKpi("TotalRaces", "Total races", 240, 90);
        AddKpi("WinRatePercent", "Win rate (%)", 460, 90);
        AddKpi("MostUsedCar", "Most-used car", 680, 90);
        AddKpi("CupCompletionPercent", "Cup completion (%)", 20, 170);
        AddKpi("VerifiedPlayers", "Verified players", 240, 170);
        AddKpi("CoinsInCirculation", "Coins in circulation", 460, 170);

        FormUi.AddLabel(this, "Races per day (last 7 days)", 20, 255, 10.5f, true);
        _gridDays = FormUi.AddGrid(this, 20, 285, 420, 260);
        FormUi.AddLabel(this, "Average finish time by track", 460, 255, 10.5f, true);
        _gridTracks = FormUi.AddGrid(this, 460, 285, 420, 260);

        var btnRefresh = FormUi.AddButton(this, "Refresh", 20, 580, 150);
        var btnClose = FormUi.AddButton(this, "Back", 185, 580, 150, primary: false);
        btnRefresh.Click += (s, e) => LoadData();
        btnClose.Click += (s, e) => Close();

        LoadData();
    }

    private void AddKpi(string key, string caption, int x, int y)
    {
        FormUi.AddLabel(this, caption, x, y, 9.5f, muted: true);
        _kpis[key] = FormUi.AddLabel(this, "-", x, y + 22, 18f, true);
    }

    private void LoadData()
    {
        try
        {
            DataRow k = _repo.GetKpis();
            foreach (var pair in _kpis)
                pair.Value.Text = Convert.ToString(k[pair.Key]) ?? "-";

            _gridDays.DataSource = _repo.GetRacesPerDay();
            FormUi.Header(_gridDays, "RaceDate", "Date");
            FormUi.Header(_gridDays, "TotalRaces", "Races");
            if (_gridDays.Columns.Contains("RaceDate"))
                _gridDays.Columns["RaceDate"]!.DefaultCellStyle.Format = "yyyy-MM-dd";

            _gridTracks.DataSource = _repo.GetAvgTimeByTrack();
            FormUi.Header(_gridTracks, "TrackName", "Track");
            FormUi.Header(_gridTracks, "RaceCount", "Races");
            FormUi.Header(_gridTracks, "AvgTimeSec", "Avg time (s)");
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load the dashboard: " + ex.Message);
        }
    }
}

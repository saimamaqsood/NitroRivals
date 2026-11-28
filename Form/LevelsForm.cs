using System.Data;
using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class LevelsForm : Form
{
    private readonly RaceRepository _repo = new();
    private readonly DataGridView _grid;

    public LevelsForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Levels", 900, 520);

        FormUi.AddLabel(this, "CAREER LEVELS", 20, 15, 20f, true);
        FormUi.AddLabel(this, "Finish in the top 3 to unlock the next level.", 22, 52, 9.5f, muted: true);

        _grid = FormUi.AddGrid(this, 20, 85, 860, 320);
        var btnRace = FormUi.AddButton(this, "Race selected level", 20, 425, 200);
        var btnClose = FormUi.AddButton(this, "Back", 235, 425, 150, primary: false);

        btnRace.Click += (s, e) => RaceSelected();
        _grid.CellDoubleClick += (s, e) => RaceSelected();
        btnClose.Click += (s, e) => Close();

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = _repo.GetLevels(Session.Id);
            FormUi.HideColumns(_grid, "LevelId", "TrackId", "RewardRankPts", "TimesCompleted");
            FormUi.Header(_grid, "LevelNumber", "Level");
            FormUi.Header(_grid, "TrackName", "Track");
            FormUi.Header(_grid, "LengthMeters", "Length (m)");
            FormUi.Header(_grid, "CornerCount", "Corners");
            FormUi.Header(_grid, "HurdleCount", "Hurdles");
            FormUi.Header(_grid, "AiDifficulty", "AI");
            FormUi.Header(_grid, "RewardCoins", "Reward");
            FormUi.Header(_grid, "IsUnlocked", "Unlocked");
            FormUi.Header(_grid, "BestPosition", "Best pos.");
            FormUi.Header(_grid, "BestTimeSec", "Best time (s)");
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load levels: " + ex.Message);
        }
    }

    private void RaceSelected()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a level first."); return; }
        if (!(bool)row["IsUnlocked"]) { FormUi.Error("This level is locked."); return; }

        var request = new RaceRequest
        {
            Mode = RaceModes.Level,
            Title = $"Level {row["LevelNumber"]} - {row["TrackName"]}",
            TrackId = (int)row["TrackId"],
            TrackName = (string)row["TrackName"],
            LengthMeters = Convert.ToDouble(row["LengthMeters"]),
            CornerCount = (int)row["CornerCount"],
            HurdleCount = (int)row["HurdleCount"],
            AiDifficulty = (int)row["AiDifficulty"],
            LevelNumber = (int)row["LevelNumber"],
            LevelId = (int)row["LevelId"]
        };

        RaceForm.Launch(this, request);
        LoadData();
    }
}

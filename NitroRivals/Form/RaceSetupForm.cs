using System.Data;
using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

/// <summary>Pick a track (and AI difficulty) for a quick race or a solo time trial.</summary>
public class RaceSetupForm : Form
{
    private readonly string _mode;
    private readonly ComboBox _cmbTrack;
    private readonly NumericUpDown _numAi;
    private readonly Label _lblTrackInfo;
    private readonly List<DataRow> _tracks = new();

    public RaceSetupForm(string mode)
    {
        _mode = mode;
        bool trial = mode == RaceModes.TimeTrial;

        FormUi.Setup(this, trial ? "Nitro Rivals - Time Trial" : "Nitro Rivals - Quick Race", 420, 360);
        FormUi.AddLabel(this, trial ? "TIME TRIAL" : "QUICK RACE", 30, 20, 20f, true);
        FormUi.AddLabel(this, trial ? "Race the clock alone." : "Race 3 AI drivers.", 32, 62, 9.5f, muted: true);

        FormUi.AddLabel(this, "Track", 30, 100);
        _cmbTrack = FormUi.AddCombo(this, 30, 125, 360);
        _lblTrackInfo = FormUi.AddLabel(this, "", 30, 160, 9.5f, muted: true);

        _numAi = FormUi.AddNumeric(this, 30, 225, 100, 1, 10, 5);
        if (!trial)
            FormUi.AddLabel(this, "AI difficulty (1-10)", 30, 197);
        else
            _numAi.Visible = false;

        var btnStart = FormUi.AddButton(this, "START RACE", 30, 275, 360);
        var btnClose = FormUi.AddButton(this, "Back", 30, 320, 360, 32, primary: false);

        _cmbTrack.SelectedIndexChanged += (s, e) => ShowTrackInfo();
        btnStart.Click += (s, e) => Start();
        btnClose.Click += (s, e) => Close();

        LoadTracks();
    }

    private void LoadTracks()
    {
        try
        {
            foreach (DataRow row in new RaceRepository().GetTracks().Rows)
            {
                _tracks.Add(row);
                _cmbTrack.Items.Add(row["TrackName"]!);
            }
            if (_cmbTrack.Items.Count > 0) _cmbTrack.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load tracks: " + ex.Message);
        }
    }

    private void ShowTrackInfo()
    {
        if (_cmbTrack.SelectedIndex < 0) return;
        DataRow t = _tracks[_cmbTrack.SelectedIndex];
        _lblTrackInfo.Text = $"{t["LengthMeters"]} m  |  {t["CornerCount"]} corners  |  {t["SurfaceType"]}";
    }

    private void Start()
    {
        if (_cmbTrack.SelectedIndex < 0) { FormUi.Error("Choose a track."); return; }
        DataRow t = _tracks[_cmbTrack.SelectedIndex];

        double length = Convert.ToDouble(t["LengthMeters"]);
        int difficulty = _mode == RaceModes.TimeTrial ? 1 : (int)_numAi.Value;

        var request = new RaceRequest
        {
            Mode = _mode,
            Title = (_mode == RaceModes.TimeTrial ? "Time Trial - " : "Quick Race - ") + t["TrackName"],
            TrackId = (int)t["TrackId"],
            TrackName = (string)t["TrackName"],
            LengthMeters = length,
            CornerCount = (int)t["CornerCount"],
            HurdleCount = (int)(length / 250),
            AiDifficulty = difficulty,
            LevelNumber = difficulty
        };

        RaceForm.Launch(this, request);
    }
}

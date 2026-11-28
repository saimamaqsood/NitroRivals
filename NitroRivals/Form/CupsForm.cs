using System.Data;
using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class CupsForm : Form
{
    private readonly CupRepository _repo = new();
    private readonly GarageRepository _garage = new();
    private readonly DataGridView _grid;
    private readonly Label _lblCoins;

    public CupsForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Championship Cups", 900, 540);

        FormUi.AddLabel(this, "CHAMPIONSHIP CUPS", 20, 15, 20f, true);
        FormUi.AddLabel(this, "Pay the entry fee, race every round, win prize coins. Higher tiers unlock after a podium.",
            22, 52, 9.5f, muted: true);
        _lblCoins = FormUi.AddLabel(this, "Coins: 0", 720, 24, 13f, true);

        _grid = FormUi.AddGrid(this, 20, 85, 860, 330);
        var btnEnter = FormUi.AddButton(this, "Enter cup", 20, 435, 170);
        var btnRace = FormUi.AddButton(this, "Race next round", 205, 435, 200);
        var btnClose = FormUi.AddButton(this, "Back", 420, 435, 150, primary: false);

        btnEnter.Click += (s, e) => EnterSelected();
        btnRace.Click += (s, e) => RaceNextRound();
        btnClose.Click += (s, e) => Close();

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = _repo.GetCups(Session.Id);
            FormUi.HideColumns(_grid, "CupId", "EntryId", "CurrentRound");
            FormUi.Header(_grid, "CupName", "Cup");
            FormUi.Header(_grid, "TierLevel", "Tier");
            FormUi.Header(_grid, "EntryFee", "Entry fee");
            FormUi.Header(_grid, "PrizePool", "Prize pool");
            FormUi.Header(_grid, "AiDifficulty", "AI");
            FormUi.Header(_grid, "TotalRaces", "Rounds");
            FormUi.Header(_grid, "TotalPoints", "Points");

            Session.Coins = _garage.GetCoins(Session.Id);
            _lblCoins.Text = $"Coins: {Session.Coins}";
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load cups: " + ex.Message);
        }
    }

    private void EnterSelected()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a cup first."); return; }

        try
        {
            _repo.EnterCup(Session.Id, (int)row["CupId"]);
            FormUi.Info("You are in! Press \"Race next round\" to start round 1.");
        }
        catch (Exception ex)
        {
            FormUi.Error(ex.Message);
        }
        LoadData();
    }

    private void RaceNextRound()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a cup first."); return; }
        if (row["EntryId"] == DBNull.Value) { FormUi.Error("Enter this cup first."); return; }

        int entryId = (int)row["EntryId"];
        try
        {
            DataRow info = _repo.GetCupEntry(entryId, Session.Id);
            int round = (int)info["CurrentRound"];
            int total = (int)info["TotalRaces"];

            if (round <= total)
            {
                double length = Convert.ToDouble(info["LengthMeters"]);
                int difficulty = (int)info["AiDifficulty"];

                var request = new RaceRequest
                {
                    Mode = RaceModes.Cup,
                    Title = $"{info["CupName"]} - round {round} of {total}",
                    TrackId = (int)info["TrackId"],
                    TrackName = (string)info["TrackName"],
                    LengthMeters = length,
                    CornerCount = (int)info["CornerCount"],
                    HurdleCount = (int)(length / 200),
                    AiDifficulty = difficulty,
                    LevelNumber = difficulty,
                    CupEntryId = entryId,
                    RoundNumber = round
                };

                if (RaceForm.Launch(this, request) == null)
                {
                    LoadData();
                    return;
                }
                info = _repo.GetCupEntry(entryId, Session.Id);
                round = (int)info["CurrentRound"];
            }

            if (round > total)
                FinishCup(entryId);
        }
        catch (Exception ex)
        {
            FormUi.Error(ex.Message);
        }
        LoadData();
    }

    private void FinishCup(int entryId)
    {
        DataRow result = _repo.FinishCup(entryId, Session.Id);
        int rank = (int)result["FinalRank"];
        int prize = (int)result["PrizeCoins"];

        FormUi.Info(
            $"Cup finished!\n\nPoints: {result["TotalPoints"]} / {result["MaxPoints"]}\n" +
            $"Final rank: {rank}\nPrize: {prize} coins" +
            (rank <= 3 ? "\n\nPodium! The next tier is now unlocked." : ""));
    }
}

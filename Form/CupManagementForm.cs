using System.Data;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class CupManagementForm : Form
{
    private readonly AdminRepository _repo = new();
    private readonly DataGridView _grid;
    private readonly TextBox _txtName;
    private readonly ComboBox _cmbTier;
    private readonly NumericUpDown _numFee, _numPrize, _numAi, _numRounds;

    public CupManagementForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Cup Management", 900, 640);

        FormUi.AddLabel(this, "CUP MANAGEMENT", 20, 15, 20f, true);
        _grid = FormUi.AddGrid(this, 20, 70, 860, 250);

        var btnRemove = FormUi.AddButton(this, "Remove selected cup", 20, 335, 210, primary: false);
        btnRemove.Click += (s, e) => Remove();

        FormUi.AddLabel(this, "Create a new cup", 20, 395, 12f, true);

        FormUi.AddLabel(this, "Name", 20, 430);
        _txtName = FormUi.AddTextBox(this, 20, 455, 250);

        FormUi.AddLabel(this, "Tier", 290, 430);
        _cmbTier = FormUi.AddCombo(this, 290, 455, 120);
        _cmbTier.Items.AddRange(new object[] { "1 - Bronze", "2 - Silver", "3 - Gold" });
        _cmbTier.SelectedIndex = 0;

        FormUi.AddLabel(this, "Entry fee", 430, 430);
        _numFee = FormUi.AddNumeric(this, 430, 455, 110, 0, 100000, 100);

        FormUi.AddLabel(this, "Prize pool", 560, 430);
        _numPrize = FormUi.AddNumeric(this, 560, 455, 110, 0, 1000000, 600);

        FormUi.AddLabel(this, "AI (1-10)", 690, 430);
        _numAi = FormUi.AddNumeric(this, 690, 455, 80, 1, 10, 3);

        FormUi.AddLabel(this, "Rounds (3-5)", 790, 430);
        _numRounds = FormUi.AddNumeric(this, 790, 455, 80, 3, 5, 3);

        var btnCreate = FormUi.AddButton(this, "Create cup", 20, 510, 170);
        var btnClose = FormUi.AddButton(this, "Back", 205, 510, 150, primary: false);
        btnCreate.Click += (s, e) => Create();
        btnClose.Click += (s, e) => Close();

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = _repo.GetAllCups();
            FormUi.HideColumns(_grid, "CupId");
            FormUi.Header(_grid, "CupName", "Cup");
            FormUi.Header(_grid, "TierLevel", "Tier");
            FormUi.Header(_grid, "EntryFee", "Entry fee");
            FormUi.Header(_grid, "PrizePool", "Prize pool");
            FormUi.Header(_grid, "AiDifficulty", "AI");
            FormUi.Header(_grid, "TotalRaces", "Rounds");
            FormUi.Header(_grid, "IsActive", "Active");
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load cups: " + ex.Message);
        }
    }

    private void Create()
    {
        try
        {
            _repo.CreateCup(Session.Id, _txtName.Text, _cmbTier.SelectedIndex + 1,
                (int)_numFee.Value, (int)_numPrize.Value, (int)_numAi.Value, (int)_numRounds.Value);
            _txtName.Clear();
            FormUi.Info("Cup created.");
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not create the cup: " + ex.Message);
        }
        LoadData();
    }

    private void Remove()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a cup first."); return; }

        try
        {
            string result = _repo.DeleteCup((int)row["CupId"]);
            FormUi.Info(result == "Deleted"
                ? "The cup was deleted."
                : "The cup already has entries, so it was deactivated instead.");
        }
        catch (Exception ex)
        {
            FormUi.Error(ex.Message);
        }
        LoadData();
    }
}

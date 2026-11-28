using System.Data;
using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class GarageForm : Form
{
    private readonly GarageRepository _repo = new();
    private readonly DataGridView _grid;
    private readonly Label _lblCoins;
    private readonly Label _lblInfo;
    private readonly Button _btnEquip, _btnEngine, _btnTire, _btnNitro, _btnSell;

    public GarageForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Garage", 900, 600);

        FormUi.AddLabel(this, "GARAGE", 20, 15, 20f, true);
        _lblCoins = FormUi.AddLabel(this, "Coins: 0", 720, 24, 13f, true);

        _grid = FormUi.AddGrid(this, 20, 70, 860, 220);
        _lblInfo = FormUi.AddLabel(this, "", 20, 305, 10.5f);

        _btnEquip  = FormUi.AddButton(this, "Equip car", 20, 470, 150);
        _btnEngine = FormUi.AddButton(this, "Upgrade Engine", 185, 470, 170);
        _btnTire   = FormUi.AddButton(this, "Upgrade Tires", 370, 470, 170);
        _btnNitro  = FormUi.AddButton(this, "Upgrade Nitro", 555, 470, 170);
        _btnSell   = FormUi.AddButton(this, "Sell car", 740, 470, 140, primary: false);
        var btnClose = FormUi.AddButton(this, "Back", 20, 530, 150, primary: false);

        _grid.SelectionChanged += (s, e) => UpdatePreview();
        _btnEquip.Click  += (s, e) => Act(r => _repo.EquipCar(Session.Id, (int)r["PlayerCarId"]));
        _btnEngine.Click += (s, e) => Act(r => _repo.UpgradeCar(Session.Id, (int)r["PlayerCarId"], "Engine"));
        _btnTire.Click   += (s, e) => Act(r => _repo.UpgradeCar(Session.Id, (int)r["PlayerCarId"], "Tire"));
        _btnNitro.Click  += (s, e) => Act(r => _repo.UpgradeCar(Session.Id, (int)r["PlayerCarId"], "Nitro"));
        _btnSell.Click   += (s, e) => Sell();
        btnClose.Click   += (s, e) => Close();

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = _repo.GetGarage(Session.Id);
            FormUi.HideColumns(_grid, "PlayerCarId", "CarId", "Price", "BaseSpeed", "BaseHandling",
                "BaseNitro", "EngineCost", "TireCost", "NitroCost");
            FormUi.Header(_grid, "ModelName", "Car");
            FormUi.Header(_grid, "VehicleClass", "Class");
            FormUi.Header(_grid, "EngineLevel", "Engine");
            FormUi.Header(_grid, "TireLevel", "Tires");
            FormUi.Header(_grid, "NitroLevel", "Nitro");
            FormUi.Header(_grid, "IsEquipped", "Equipped");

            Session.Coins = _repo.GetCoins(Session.Id);
            _lblCoins.Text = $"Coins: {Session.Coins}";
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load your garage: " + ex.Message);
        }
        UpdatePreview();
    }

    private void Act(Action<DataRow> action)
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a car first."); return; }

        try { action(row); }
        catch (Exception ex) { FormUi.Error(ex.Message); }
        LoadData();
    }

    private void Sell()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a car first."); return; }

        int refund = Convert.ToInt32(row["Price"]) / 2;
        var answer = MessageBox.Show(
            $"Sell the {row["ModelName"]} for {refund} coins?\nIts upgrades are lost.",
            "Nitro Rivals", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes) return;

        Act(r => _repo.SellCar(Session.Id, (int)r["PlayerCarId"]));
    }

    /// <summary>Shows what each upgrade would change, using a copy of the car (copy constructor).</summary>
    private void UpdatePreview()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        bool has = row != null;
        _btnEquip.Enabled = _btnSell.Enabled = has;
        _btnEngine.Enabled = _btnTire.Enabled = _btnNitro.Enabled = has;

        if (row == null) { _lblInfo.Text = ""; return; }

        Vehicle car = VehicleFactory.FromRow(row);
        _btnEngine.Enabled = car.EngineLevel < Vehicle.MaxUpgradeLevel;
        _btnTire.Enabled   = car.TireLevel   < Vehicle.MaxUpgradeLevel;
        _btnNitro.Enabled  = car.NitroLevel  < Vehicle.MaxUpgradeLevel;

        _lblInfo.Text =
            $"{car}\n" +
            $"Top speed {car.TopSpeed * 3.6:F0} km/h   |   Corner speed {car.CalculateCornerSpeed(car.TopSpeed) * 3.6:F0} km/h   |   Nitro {car.NitroSeconds:F1} s\n\n" +
            Describe(car, row, "Engine (top speed)", "EngineCost", c => c.EngineLevel, c => c.EngineLevel++,
                c => c.TopSpeed * 3.6, "km/h") + "\n" +
            Describe(car, row, "Tires (corner speed)", "TireCost", c => c.TireLevel, c => c.TireLevel++,
                c => c.CalculateCornerSpeed(c.TopSpeed) * 3.6, "km/h") + "\n" +
            Describe(car, row, "Nitro (duration)", "NitroCost", c => c.NitroLevel, c => c.NitroLevel++,
                c => c.NitroSeconds, "s");
    }

    private static string Describe(Vehicle car, DataRow row, string label, string costColumn,
        Func<Vehicle, int> level, Action<Vehicle> upgrade, Func<Vehicle, double> stat, string unit)
    {
        if (level(car) >= Vehicle.MaxUpgradeLevel)
            return $"{label}: MAX level";

        Vehicle preview = car.Clone();      // copy constructor: the real car is untouched
        upgrade(preview);
        return $"{label}: {stat(car):F1} -> {stat(preview):F1} {unit}   (cost {row[costColumn]} coins)";
    }
}

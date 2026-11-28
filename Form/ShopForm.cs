using System.Data;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class ShopForm : Form
{
    private readonly GarageRepository _repo = new();
    private readonly DataGridView _grid;
    private readonly Label _lblCoins;

    public ShopForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Shop", 900, 520);

        FormUi.AddLabel(this, "CAR SHOP", 20, 15, 20f, true);
        _lblCoins = FormUi.AddLabel(this, "Coins: 0", 720, 24, 13f, true);

        _grid = FormUi.AddGrid(this, 20, 70, 860, 330);
        var btnBuy = FormUi.AddButton(this, "Buy selected car", 20, 420, 200);
        var btnClose = FormUi.AddButton(this, "Back", 235, 420, 150, primary: false);

        btnBuy.Click += (s, e) => Buy();
        btnClose.Click += (s, e) => Close();

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _grid.DataSource = _repo.GetShopCars(Session.Id);
            FormUi.HideColumns(_grid, "CarId");
            FormUi.Header(_grid, "ModelName", "Car");
            FormUi.Header(_grid, "VehicleClass", "Class");
            FormUi.Header(_grid, "BaseSpeed", "Speed");
            FormUi.Header(_grid, "BaseHandling", "Handling");
            FormUi.Header(_grid, "BaseNitro", "Nitro");

            Session.Coins = _repo.GetCoins(Session.Id);
            _lblCoins.Text = $"Coins: {Session.Coins}";
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not load the shop: " + ex.Message);
        }
    }

    private void Buy()
    {
        DataRow? row = FormUi.SelectedRow(_grid);
        if (row == null) { FormUi.Error("Select a car first."); return; }
        if ((bool)row["Owned"]) { FormUi.Error("You already own this car."); return; }

        try
        {
            _repo.BuyCar(Session.Id, (int)row["CarId"]);
            FormUi.Info($"You bought the {row["ModelName"]}! Equip it in the Garage.");
        }
        catch (Exception ex)
        {
            FormUi.Error(ex.Message);
        }
        LoadData();
    }
}

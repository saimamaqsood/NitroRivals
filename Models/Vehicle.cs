namespace NitroRivals.Models;

public class Vehicle
{
    public int VehicleId { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int UnlockPriceCoins { get; set; }
    public decimal TopSpeed { get; set; }
    public decimal Handling { get; set; }
    public decimal NitroCapacity { get; set; }
}
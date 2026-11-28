namespace NitroRivals.Models;

public class Driver
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int SkillLevel { get; set; }
    public decimal Aggression { get; set; }
    public decimal BaseSpeed { get; set; }
    public int? AssignedTierId { get; set; }
}
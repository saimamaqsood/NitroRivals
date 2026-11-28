using System.Data;
using NitroRivals.Models;

namespace NitroRivals.Services;


/// <summary>Builds the right Vehicle subclass from a Cars table row.</summary>
public static class VehicleFactory
{
    public static Vehicle Create(string vehicleClass, string modelName,
        double baseSpeed, double baseHandling, double baseNitro,
        int engineLevel = 0, int tireLevel = 0, int nitroLevel = 0)
    {
        Vehicle vehicle = vehicleClass switch
        {
            "Sports" => (Vehicle)new SportsCar(modelName, baseSpeed, baseHandling, baseNitro),
            "Muscle" => new MuscleCar(modelName, baseSpeed, baseHandling, baseNitro),
            "Truck"  => new Truck(modelName, baseSpeed, baseHandling, baseNitro),
            _ => throw new ArgumentException($"Unknown vehicle class '{vehicleClass}'.")
        };

        vehicle.EngineLevel = engineLevel;
        vehicle.TireLevel = tireLevel;
        vehicle.NitroLevel = nitroLevel;
        return vehicle;
    }
    public static Vehicle FromRow(DataRow row)
    {
        string vehicleClass = row.Table.Columns.Contains("VehicleClass") ? row["VehicleClass"]?.ToString() ?? "Sports" : "Sports";
        string modelName = row.Table.Columns.Contains("ModelName") ? row["ModelName"]?.ToString() ?? "Car" : "Car";
        double baseSpeed = row.Table.Columns.Contains("BaseSpeed") ? Convert.ToDouble(row["BaseSpeed"]) : 200.0;
        double baseHandling = row.Table.Columns.Contains("BaseHandling") ? Convert.ToDouble(row["BaseHandling"]) : 75.0;
        double baseNitro = row.Table.Columns.Contains("BaseNitro") ? Convert.ToDouble(row["BaseNitro"]) : 50.0;
        int engineLevel = row.Table.Columns.Contains("EngineLevel") ? Convert.ToInt32(row["EngineLevel"]) : 0;
        int tireLevel = row.Table.Columns.Contains("TireLevel") ? Convert.ToInt32(row["TireLevel"]) : 0;
        int nitroLevel = row.Table.Columns.Contains("NitroLevel") ? Convert.ToInt32(row["NitroLevel"]) : 0;

        return Create(vehicleClass, modelName, baseSpeed, baseHandling, baseNitro, engineLevel, tireLevel, nitroLevel);
    }
}

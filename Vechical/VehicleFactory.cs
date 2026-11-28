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
}

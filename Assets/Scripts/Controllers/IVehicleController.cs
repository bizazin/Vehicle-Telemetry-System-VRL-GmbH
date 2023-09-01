namespace Controllers
{
    public interface IVehicleController
    {
        float Speed { get; }
        float Rpm { get; }
        bool IsEngineActivate { get; }
        string ActiveGear { get; }
        int TransmissionOperatingMode { get; }
        float ProximityToClosestCar { get; }
    }
}
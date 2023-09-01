namespace Controllers
{
    public interface IDashboardController
    {
        void SetSpeed(float currentSpeed);
        void SetEngineRpm(float currentRpm);
        void SetIsEngineActive(bool isActive);
        void SetActiveGear(string activeGear);
        void SetTransmissionOperatingMode(int gearboxMode);
        void SetClosestVehicleDistance(float distance);
    }
}
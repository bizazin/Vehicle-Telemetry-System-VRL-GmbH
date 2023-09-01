using Core.Abstracts;
using Views;
using Zenject;

namespace Controllers.Impls
{
    public class DashboardController : Controller<DashboardView>, IInitializable, IDashboardController
    {
        
        public void Initialize()
        {
            
        }

        public void SetSpeed(float currentSpeed) => View.SetSpeed(currentSpeed);

        public void SetEngineRpm(float currentRpm) => View.SetEngineRpm(currentRpm);

        public void SetIsEngineActive(bool isActive) => View.SetIsEngineActive(isActive);

        public void SetActiveGear(string activeGear) => View.SetActiveGear(activeGear);

        public void SetTransmissionOperatingMode(int gearboxMode) => View.SetTransmissionOperatingMode(gearboxMode);

        public void SetClosestVehicleDistance(float distance) => View.SetClosestVehicleDistance(distance);
    }
}
using Core.Abstracts;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class DashboardView : View
    {
        [SerializeField] private Text _speedValueText;
        [SerializeField] private Text _rpmValueText;
        [SerializeField] private Toggle _engineActiveToggle;
        [SerializeField] private Text _activeGearValueText;
        [SerializeField] private Text _transmissionModeValueText;
        [SerializeField] private Text _closestVehicleValueText;

        public void SetSpeed(float currentSpeed) => _speedValueText.text = currentSpeed.ToString();

        public void SetEngineRpm(float currentRpm) => _rpmValueText.text = currentRpm.ToString();

        public void SetIsEngineActive(bool isActive) => _engineActiveToggle.isOn = isActive;

        public void SetActiveGear(string activeGear) => _activeGearValueText.text = activeGear;
        
        public void SetTransmissionOperatingMode(int gearboxMode) => _transmissionModeValueText.text = gearboxMode.ToString();
        
        public void SetClosestVehicleDistance(float distance) => _closestVehicleValueText.text = distance.ToString();
    }
}
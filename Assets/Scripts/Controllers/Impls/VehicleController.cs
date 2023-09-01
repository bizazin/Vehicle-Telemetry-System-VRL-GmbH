using System;
using Core.Abstracts;
using Databases;
using Extensions;
using UniRx;
using VehiclePhysics;
using Views;
using Zenject;

namespace Controllers.Impls
{
    public class VehicleController : Controller<VehicleView>, IInitializable, IDisposable, IVehicleController
    {
        private readonly IDashboardController _dashboardController;
        private readonly IVehicleSettingsDatabase _vehicleSettingsDatabase;
        private readonly CompositeDisposable _disposable = new();

        private bool _isInputEnabled;
        private bool _isEngineStalled;
        private bool _switchingGear;
        private int _gearboxMode;
        private Action<float> _closestVehicleDistanceHandler;

        public float Speed { get; private set; }
        public float Rpm { get; private set; }
        public bool IsEngineActivate { get; private set; }
        public string ActiveGear { get; private set; }
        public int TransmissionOperatingMode { get; private set; }
        public float ProximityToClosestCar { get; private set; }
        

        public VehicleController
        (
            IDashboardController dashboardController,
            IVehicleSettingsDatabase vehicleSettingsDatabase
        )
        {
            _dashboardController = dashboardController;
            _vehicleSettingsDatabase = vehicleSettingsDatabase;
        }

        public void Initialize()
        {
            var fieldOfViewRadiusM = _vehicleSettingsDatabase.Settings.FieldOfViewRadiusM;
            View.SetupFieldOfView(fieldOfViewRadiusM);
            _dashboardController.SetClosestVehicleDistance(fieldOfViewRadiusM);
            
            #region Velocity

            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle, VehicleData.Speed).MSToKmH())
                .DistinctUntilChanged()
                .Subscribe(currentSpeed =>
                {
                    Speed = currentSpeed;
                    _dashboardController.SetSpeed(currentSpeed);
                })
                .AddTo(_disposable);

            #endregion

            #region EngineRpm

            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000)
                .DistinctUntilChanged()
                .Subscribe(currentRpm =>
                {
                    Rpm = currentRpm;
                    _dashboardController.SetEngineRpm(currentRpm);
                })
                .AddTo(_disposable);

            #endregion

            #region IsEngineActive

            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Input)[InputData.Key] >= 0)
                .DistinctUntilChanged()
                .Subscribe(isInputEnabled =>
                {
                    _isInputEnabled = isInputEnabled;
                    IsEngineActivate = !(isInputEnabled && _isEngineStalled);
                    _dashboardController.SetIsEngineActive(IsEngineActivate);
                })
                .AddTo(_disposable);
            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.EngineStalled] != 0)
                .DistinctUntilChanged()
                .Subscribe(isEngineStalled =>
                {
                    _isEngineStalled = isEngineStalled;
                    IsEngineActivate = !(_isInputEnabled && _isEngineStalled);
                    _dashboardController.SetIsEngineActive(IsEngineActivate);
                })
                .AddTo(_disposable);

            #endregion

            #region Gear

            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxMode])
                .DistinctUntilChanged()
                .Subscribe(gearboxMode =>
                {
                    _gearboxMode = gearboxMode;
                    SetGear();
                })
                .AddTo(_disposable);
            View.Base
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxGear])
                .DistinctUntilChanged()
                .Subscribe(gearId =>
                {
                    TransmissionOperatingMode = gearId;
                    _dashboardController.SetTransmissionOperatingMode(gearId);

                    SetGear();
                })
                .AddTo(_disposable);
            View.Base
                .ObserveEveryValueChanged(
                    vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxShifting] != 0)
                .DistinctUntilChanged()
                .Subscribe(switchingGear =>
                {
                    _switchingGear = switchingGear;
                    SetGear();
                })
                .AddTo(_disposable);

            #endregion

            #region Proximity to Next Car

            _closestVehicleDistanceHandler = dist =>
            {
                ProximityToClosestCar = dist;
                _dashboardController.SetClosestVehicleDistance(dist);
            };

            View.FieldOfView.OnClosestVehicleDistM += _closestVehicleDistanceHandler;

            #endregion
        }

        private void SetGear()
        {
            string activeGear = "-";
            switch (_gearboxMode)
            {
                case 0: // M
                    if (TransmissionOperatingMode == 0)
                        activeGear = _switchingGear ? " " : "N";
                    else if (TransmissionOperatingMode > 0)
                        activeGear = TransmissionOperatingMode.ToString();
                    else
                        activeGear = TransmissionOperatingMode == -1 ? "R" : "R" + -TransmissionOperatingMode;
                    break;

                case 1: // P
                    activeGear = "P";
                    break;

                case 2: // R
                    activeGear = TransmissionOperatingMode < -1 ? "R" + -TransmissionOperatingMode : "R";
                    break;

                case 3: // N
                    activeGear = "N";
                    break;

                case 4: // D
                case 5: // L
                    if (TransmissionOperatingMode > 0)
                        activeGear = (_gearboxMode == 4 ? "D" : "L") + TransmissionOperatingMode;
                    else
                        activeGear = _gearboxMode == 4 ? "D" : "L";
                    break;
            }

            UpdateActiveGear(activeGear);
        }
        
        private void UpdateActiveGear(string gear)
        {
            ActiveGear = gear;
            _dashboardController.SetActiveGear(gear);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            View.FieldOfView.OnClosestVehicleDistM -= _closestVehicleDistanceHandler;
        }
    }
}
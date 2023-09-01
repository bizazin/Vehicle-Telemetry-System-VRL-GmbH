using System;
using Extensions;
using UniRx;
using UnityEngine;
using VehiclePhysics;

namespace Behaviours
{
    public class VehicleView : MonoBehaviour
    {
        [SerializeField] private FieldOfViewBehaviour _fieldOfView;
        [SerializeField] private VehicleBase _vehicleBase;
        [SerializeField] private DashboardView _dashboard;
        private bool _isInputEnabled;
        private bool _isEngineStalled;
        private bool _switchingGear;
        private int _gearboxMode;
        private Action<float> _closestVehicleDistanceHandler;
        private readonly CompositeDisposable _disposable = new();

        public float Speed { get; private set; }
        public float Rpm { get; private set; }
        public bool IsEngineActivate { get; private set; }
        public int TransmissionOperatingMode { get; private set; }
        public float ProximityToClosestCar { get; private set; }


        private void OnEnable()
        {
            #region Velocity
            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle, VehicleData.Speed).MSToKmH())
                .DistinctUntilChanged()
                .Subscribe(currentSpeed =>
                {
                    Speed = currentSpeed;
                    _dashboard.SetSpeed(currentSpeed);
                })
                .AddTo(_disposable);
            #endregion

            #region EngineRpm

            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle, VehicleData.EngineRpm) / 1000)
                .DistinctUntilChanged()
                .Subscribe(currentRpm =>
                {
                    Rpm = currentRpm;
                    _dashboard.SetEngineRpm(currentRpm);
                })
                .AddTo(_disposable);


            #endregion

            #region IsEngineActive

            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Input)[InputData.Key] >= 0)
                .DistinctUntilChanged()
                .Subscribe(isInputEnabled =>
                {
                    _isInputEnabled = isInputEnabled;
                    IsEngineActivate = !(isInputEnabled && _isEngineStalled);
                    _dashboard.SetIsEngineActive(IsEngineActivate);
                })
                .AddTo(_disposable);
            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.EngineStalled] != 0)
                .DistinctUntilChanged()
                .Subscribe(isEngineStalled =>
                {
                    _isEngineStalled = isEngineStalled;
                    IsEngineActivate = !(_isInputEnabled && _isEngineStalled);
                    _dashboard.SetIsEngineActive(IsEngineActivate);
                })
                .AddTo(_disposable);

            #endregion

            #region Gear

            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxMode])
                .DistinctUntilChanged()
                .Subscribe(gearboxMode =>
                {
                    _gearboxMode = gearboxMode;
                    SetGear();
                })
                .AddTo(_disposable);
            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxGear])
                .DistinctUntilChanged()
                .Subscribe(gearId =>
                {
                    TransmissionOperatingMode = gearId;
                    _dashboard.SetTransmissionOperatingMode(gearId);

                    SetGear();
                })
                .AddTo(_disposable);
            _vehicleBase
                .ObserveEveryValueChanged(vehicle => vehicle.data.Get(Channel.Vehicle)[VehicleData.GearboxShifting] != 0)
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
                _dashboard.SetClosestVehicleDistance(dist);
            };

            _fieldOfView.OnClosestVehicleDistM += _closestVehicleDistanceHandler;

                #endregion
        }

        private void SetGear()
        {
            switch (_gearboxMode)
            {
                case 0:		// M
                    if (TransmissionOperatingMode == 0)
                        _dashboard.SetActiveGear(_switchingGear ? " " : "N");
                    else
                    if (TransmissionOperatingMode > 0)
                        _dashboard.SetActiveGear(TransmissionOperatingMode.ToString());
                    else
                    {
                        if (TransmissionOperatingMode == -1)
                            _dashboard.SetActiveGear("R");
                        else
                            _dashboard.SetActiveGear("R" + -TransmissionOperatingMode);
                    }
                    break;

                case 1:		// P
                    _dashboard.SetActiveGear("P");
                    break;

                case 2:		// R
                    if (TransmissionOperatingMode < -1)
                        _dashboard.SetActiveGear("R" + -TransmissionOperatingMode);
                    else
                        _dashboard.SetActiveGear("R");
                    break;

                case 3:		// N
                    _dashboard.SetActiveGear("N");
                    break;

                case 4:		// D
                    if (TransmissionOperatingMode > 0)
                        _dashboard.SetActiveGear("D" + TransmissionOperatingMode);
                    else
                        _dashboard.SetActiveGear("D");
                    break;

                case 5:		// L
                    if (TransmissionOperatingMode > 0)
                        _dashboard.SetActiveGear("L" + TransmissionOperatingMode);
                    else
                        _dashboard.SetActiveGear("L");
                    break;

                default:
                    _dashboard.SetActiveGear("-");
                    break;
            }

        }

        private void OnDisable()
        {
            _disposable.Clear();
            _fieldOfView.OnClosestVehicleDistM -= _closestVehicleDistanceHandler;
        }
    }
}
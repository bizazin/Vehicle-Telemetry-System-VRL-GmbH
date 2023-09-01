using Models;
using UnityEngine;
using Zenject;

namespace Databases.Impls
{
    
    [CreateAssetMenu(menuName = "Databases/VehicleSettingsDatabase", fileName = "VehicleSettingsDatabase")]
    public class VehicleSettingsDatabase : ScriptableObjectInstaller, IVehicleSettingsDatabase
    {
        [SerializeField] private VehicleSettingsVo _vehicleSettings;

        public VehicleSettingsVo Settings => _vehicleSettings;

    }
}
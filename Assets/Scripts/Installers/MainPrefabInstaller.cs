using Behaviours;
using Controllers;
using Controllers.Impls;
using Databases;
using Databases.Impls;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using Views;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(menuName = "Installers/MainPrefabInstaller", fileName = "MainPrefabInstaller")]
    public class MainPrefabInstaller : ScriptableObjectInstaller
    {
        [Header("Databases")] 
        [SerializeField] private VehicleSettingsDatabase _vehicleSettingsDatabase;
        
        [Header("Canvas")] 
        [SerializeField] private Canvas _canvas;
        
        [Header("Ui Views")]
        [SerializeField] private DashboardView _dashboardView;

        public override void InstallBindings()
        {
            BindDatabases();
            BindUiViews();
        }

        private void BindDatabases()
        {
            Container.Bind<IVehicleSettingsDatabase>().FromInstance(_vehicleSettingsDatabase).AsSingle();
        }

        private void BindUiViews()
        {
            Container.Bind<CanvasScaler>().FromComponentInNewPrefab(_canvas).AsSingle();
            var parent = Container.Resolve<CanvasScaler>().transform;
            
            Container.BindUiView<DashboardController, DashboardView>(_dashboardView, parent);
        }
    }
}
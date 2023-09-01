using Behaviours;
using Controllers;
using Controllers.Impls;
using Extensions;
using UnityEngine;
using Views;
using Zenject;
using Object = UnityEngine.Object;

namespace Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private VehicleView _vehicleView;
        [SerializeField] private CameraView _cameraView;

        public override void InstallBindings()
        {
            Container.BindView<VehicleController, VehicleView>(_vehicleView, _spawnPoint);
            BindPrefab(_cameraView).NonLazy();
        }

        private ConcreteIdArgConditionCopyNonLazyBinder BindPrefab<TContent>(TContent prefab)
            where TContent : Object =>
            Container.BindInterfacesAndSelfTo<TContent>()
                .FromComponentInNewPrefab(prefab)
                .AsSingle();


    }
}
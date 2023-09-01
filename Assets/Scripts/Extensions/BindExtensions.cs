using Core.Interfaces;
using UnityEngine;
using Zenject;

namespace Extensions
{
    public static class BindExtensions 
    {
        public static void BindView<T, TU>(this DiContainer container, Object viewPrefab, Transform spawnTransform)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            var instance = container.InstantiatePrefabForComponent<TU>(viewPrefab, spawnTransform.position,
                spawnTransform.rotation, null);
            container.Bind<TU>().FromInstance(instance).AsSingle();
        }
        
        public static void BindUiView<T, TU>(this DiContainer container, Object viewPrefab, Transform parent)
            where TU : IView
            where T : IController
        {
            container.BindInterfacesAndSelfTo<T>().AsSingle();
            container.BindInterfacesAndSelfTo<TU>()
                .FromComponentInNewPrefab(viewPrefab)
                .UnderTransform(parent).AsSingle();
        }


    }
}
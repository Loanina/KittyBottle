using Core.SceneManagement;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class InitSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject sceneLoaderPrefab;

        public override void InstallBindings()
        {
            var sceneLoaderInstance = Container.InstantiatePrefab(sceneLoaderPrefab);

            Container.Bind<ISceneLoader>()
                .FromComponentInNewPrefab(sceneLoaderPrefab)
                .AsSingle();

            Container.BindInterfacesTo<AppInitializer>().AsSingle();
        }
    }
}
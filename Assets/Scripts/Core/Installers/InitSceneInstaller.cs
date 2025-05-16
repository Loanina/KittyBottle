using Core.SceneManagement;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    [CreateAssetMenu(fileName = "InitSceneInstaller", menuName = "Game/Installers/InitSceneInstaller")]
    public class InitSceneInstaller : ScriptableObjectInstaller<InitSceneInstaller>
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
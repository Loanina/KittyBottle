using Core.SceneManagement;
using Core.UI;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private FadingPanel fadingPanel;
        [SerializeField] private LoadingScreenConfig config;
        [SerializeField] private SceneLoader sceneLoader;

        public override void InstallBindings()
        {
            Container.Bind<LoadingScreenConfig>().FromInstance(config).AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoader>().FromInstance(sceneLoader).AsSingle();
            Container.BindInterfacesAndSelfTo<LoadingScreenController>().AsSingle().WithArguments(loadingScreen, fadingPanel, config);
        }

    }
}
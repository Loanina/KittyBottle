using Scenes.GameScene.Bottle;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class BottleInstaller : MonoInstaller
    {
        [SerializeField] private Bottle bottle;
        [SerializeField] private BottleView view;
        [SerializeField] private BottleShaderConfig shaderConfig;
        [SerializeField] private PouringAnimationConfig pouringAnimationConfig;
       
        
        public override void InstallBindings()
        {
            Container.Bind<BottleView>().FromInstance(view).AsSingle();
            Container.BindInterfacesAndSelfTo<Bottle>().FromInstance(bottle).AsSingle();
            Container.Bind<BottleShaderController>().AsSingle().WithArguments(shaderConfig);
            Container.Bind<PouringAnimationController>().AsSingle().WithArguments(pouringAnimationConfig);
        }
    }
}
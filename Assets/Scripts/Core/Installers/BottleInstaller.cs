using Scenes.GameScene.Bottle;
using Scenes.GameScene.Bottle.Animation;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class BottleInstaller : MonoInstaller
    {
        [SerializeField] private Bottle bottle;
        [SerializeField] private BottleView view;
        [SerializeField] private BottleShaderConfig shaderConfig;
        [SerializeField] private BottleAnimationConfig animationConfig;
       
        
        public override void InstallBindings()
        {
            Container.Bind<BottleView>().FromInstance(view).AsSingle();
            Container.BindInterfacesAndSelfTo<Bottle>().FromInstance(bottle).AsSingle();
            Container.Bind<BottleShaderController>().AsSingle().WithArguments(shaderConfig);
            Container.Bind<BottleAnimationController>().AsSingle().WithArguments(animationConfig, transform);
        }
    }
}
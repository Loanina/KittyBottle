using Zenject;

namespace Core.InputSystem
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<InputSignal>();

            Container.BindInterfacesAndSelfTo<ClickableObject>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle().NonLazy();
        }
    }
}
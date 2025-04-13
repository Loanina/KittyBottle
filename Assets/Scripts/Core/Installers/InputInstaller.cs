using Core.InputSystem;
using Zenject;

namespace Core.Installers
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<InputSignal>();
            Container.BindInterfacesTo<InputHandler>().AsSingle();
            Container.BindInterfacesTo<InputRaycaster>().AsSingle();
        }
    }
}
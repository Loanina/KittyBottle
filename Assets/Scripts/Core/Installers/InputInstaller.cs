using Core.InputSystem;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    [CreateAssetMenu(menuName = "Game/Installers/InputInstaller")]
    public class InputInstaller : ScriptableObjectInstaller<InputInstaller>
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
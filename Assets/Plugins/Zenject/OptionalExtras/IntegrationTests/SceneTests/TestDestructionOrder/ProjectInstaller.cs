using Zenject;
using Zenject.Tests.TestDestructionOrder;

namespace Plugins.Zenject.OptionalExtras.IntegrationTests.SceneTests.TestDestructionOrder
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<FooDisposable3>().AsSingle();
        }
    }
}

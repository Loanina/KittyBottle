using System.Threading.Tasks;

namespace Core.SceneManagement
{
    public interface IAppInitializer
    {
        Task InitializeAsync();
    }
}
using System;
using System.Threading.Tasks;

namespace Core.SceneManagement
{
    public interface ISceneLoader
    {
        Task LoadSceneAsync(int sceneIndex);
        void LoadScene(int sceneIndex, Action onFadeInComplete = null, Action onFadeOutComplete = null);
    }
}
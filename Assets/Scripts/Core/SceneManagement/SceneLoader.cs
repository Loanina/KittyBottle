using System;
using System.Threading.Tasks;
using Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Core.SceneManagement
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        private ILoadingScreenController loadingScreenController;
        private LoadingScreenConfig config;

        [Inject]
        public void Construct(ILoadingScreenController loadingScreenController, LoadingScreenConfig config)
        {
            this.loadingScreenController = loadingScreenController;
            this.config = config;
        }

        public async Task LoadSceneAsync(int sceneIndex)
        {
            loadingScreenController.ShowLoadingScreen();

            var scene = SceneManager.LoadSceneAsync(sceneIndex);
            scene.allowSceneActivation = false;

            while (scene.progress < 0.9f)
            {
                await Task.Delay(config.DelayToUpdateProgressbarMS);
                loadingScreenController.UpdateProgress(scene.progress);
            }

            scene.allowSceneActivation = true;
            loadingScreenController.HideLoadingScreen();
        }

        public void LoadScene(int sceneIndex, Action onFadeInComplete = null, Action onFadeOutComplete = null)
        {
            loadingScreenController.ShowLoadingScreen();
            SceneManager.LoadScene(sceneIndex);
            onFadeInComplete?.Invoke();
            loadingScreenController.HideLoadingScreen();
            onFadeOutComplete?.Invoke();
        }
    }
}
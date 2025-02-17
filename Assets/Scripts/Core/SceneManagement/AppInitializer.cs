using UnityEngine;
using Zenject;

namespace Core.SceneManagement
{
    public class AppInitializer : IInitializable
    {
        private readonly ISceneLoader sceneLoader;

        [Inject]
        public AppInitializer(ISceneLoader sceneLoader)
        {
            this.sceneLoader = sceneLoader;
        }

        public void Initialize()
        {
            Debug.Log("Инициализация приложения начата...");
            LoadMainMenuAsync();
        }

        private async void LoadMainMenuAsync()
        {
            try
            {
                await sceneLoader.LoadSceneAsync((int)SceneType.GameScene);
                Debug.Log("Инициализация приложения завершена.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка при загрузке сцены: {ex.Message}");
            }
        }
    }
}
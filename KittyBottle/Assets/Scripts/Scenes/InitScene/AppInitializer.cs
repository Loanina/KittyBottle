using UnityEngine;
using Zenject;

public class AppInitializer : MonoBehaviour
{
    private SceneLoader _sceneLoader;
     
    [Inject]
    public void Init(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    private void Start() => _sceneLoader.LoadSceneAsync(Scene.HomeScene);
}

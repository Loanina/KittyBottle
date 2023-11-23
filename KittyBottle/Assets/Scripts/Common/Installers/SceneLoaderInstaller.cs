using UnityEngine;
using Zenject;

public class SceneLoaderInstaller : MonoInstaller
{
     [SerializeField] private SceneLoader sceneLoaderPrefab;

     public override void InstallBindings()
     {
          var sceneLoader = Container.InstantiatePrefabForComponent<SceneLoader>(sceneLoaderPrefab);
          Container.Bind<SceneLoader>().FromInstance(sceneLoader).AsSingle();
          sceneLoader.transform.SetAsLastSibling();
     }
}

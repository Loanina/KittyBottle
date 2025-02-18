using Common.DataManagement;
using Core.SavingSystem;
using Scenes.GameScene;
using Scenes.GameScene.Bottle;
using Scenes.GameScene.ColorPalette;
using Scenes.GameScene.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private BottlesContainer bottlesContainer;
        [SerializeField] private HintManager hintManager;
        [SerializeField] private ColorPaletteCollection paletteCollection;
        [SerializeField, Range(0, 1)] private int palletIndex;
        [SerializeField] private LevelCollection levelCollection;
        [SerializeField] private int coinsPerLevel = 10;

        [ContextMenu("Clear player data")]
        private void ClearPlayerData()
        {
            SaveSystem<PlayerData>.Instance.Clear();
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ColorPaletteController>().AsSingle().WithArguments(paletteCollection, palletIndex);
            Container.Bind<LevelColorMapper>().AsSingle();
            Container.Bind<LevelProvider>().AsSingle().WithArguments(levelCollection);
            Container.Bind<PlayerProgressService>().AsSingle().WithArguments(coinsPerLevel);
            Container.Bind<ISaveSystem<PlayerData>>()
                .To<SaveSystemAdapter<PlayerData>>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(bottlesContainer, hintManager);
            Debug.Log("Game scene data loaded");
        }
    }
}
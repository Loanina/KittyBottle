using Common.DataManagement;
using Common.Logging;
using Core.Hints;
using Core.SavingSystem;
using Scenes.GameScene.Bottle;
using Scenes.GameScene.Bottle.Moves;
using Scenes.GameScene.ColorPalette;
using Scenes.GameScene.Level;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField, Range(0, 1)] private int palletIndex;
        [SerializeField] private int coinsPerLevel = 10;
        [SerializeField] private HintManager hintManager;
        [SerializeField] private ColorPaletteCollection paletteCollection;
        [SerializeField] private LevelCollection levelCollection;
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private LayoutSettings layoutSettings;
        [SerializeField] private Transform bottlesParentTransform;

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
            Container.Bind<ISaveSystem<PlayerData>>().To<SaveSystemAdapter<PlayerData>>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(hintManager);
            Container.Bind<LevelCompletionChecker>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelProgressHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<BottlesController>().AsSingle();
            Container.Bind<BottlesContainer>().AsSingle().WithArguments(layoutSettings);
            Container.BindFactory<Bottle, BottleFactory>()
                .FromComponentInNewPrefab(bottlePrefab)
                .UnderTransform(bottlesParentTransform);

            Container.Bind<MovesManager>().AsSingle();
            Container.Bind<BestMoveFinder>().AsSingle();
            Container.Bind<IGameLogger>().To<GameLogger>().AsSingle();
            Debug.Log("Game scene data loaded");
        }
    }
}
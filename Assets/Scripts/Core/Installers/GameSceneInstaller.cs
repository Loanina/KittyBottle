using Common.DataManagement;
using Common.Logging;
using Core.Hints;
using Core.Hints.Moves;
using Core.SavingSystem;
using Scenes.GameScene;
using Scenes.GameScene.Bottle;
using Scenes.GameScene.ColorPalette;
using Scenes.GameScene.Hints;
using Scenes.GameScene.Level;
using Scenes.GameScene.Reward;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Core.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField, Range(0, 1)] private int palletIndex;
        [SerializeField] private HintManager hintManager;
        [SerializeField] private GameUIManager gameUIManager;
        [SerializeField] private ColorPaletteCollection paletteCollection;
        [SerializeField] private LevelCollection levelCollection;
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private LayoutSettings layoutSettings;
        [SerializeField] private Transform bottlesRoot;
        [SerializeField] private RectTransform rewardRoot;

        [Button("Clear player data")]
        private void ClearPlayerData()
        {
            SaveSystem<PlayerData>.Instance.Clear();
            Debug.Log("Player data deleted");
        }

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ColorPaletteController>().AsSingle().WithArguments(paletteCollection, palletIndex);
            Container.Bind<LevelColorMapper>().AsSingle();
            Container.Bind<LevelProvider>().AsSingle().WithArguments(levelCollection);
            Container.Bind<PlayerProgressService>().AsSingle();
            Container.Bind<ISaveSystem<PlayerData>>().To<SaveSystemAdapter<PlayerData>>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle();
            Container.Bind<LevelCompletionChecker>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelProgressHandler>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<BottlesController>().AsSingle();
            Container.Bind<BottlesContainer>().AsSingle().WithArguments(layoutSettings);
            Container.BindFactory<Bottle, BottleFactory>()
                .FromComponentInNewPrefab(bottlePrefab)
                .UnderTransform(bottlesRoot);
            Container.Bind<PouringService>().AsSingle();

            Container.Bind<HintManager>().FromInstance(hintManager).AsSingle();
            Container.Bind<MoveHistory>().AsSingle();
            Container.Bind<BestMoveFinder>().AsSingle();
            Container.Bind<IGameLogger>().To<GameLogger>().AsSingle();

            Container.Bind<RewardService>().AsSingle().WithArguments(rewardRoot);
            Container.Bind<RewardInventoryService>().AsSingle();

            Container.Bind<GameUIManager>().FromInstance(gameUIManager).AsSingle();
            Debug.Log("Game scene data loaded");
        }
    }
}
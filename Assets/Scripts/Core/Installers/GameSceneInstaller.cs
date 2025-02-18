using Common.DataManagement;
using Core.Hints;
using Core.SavingSystem;
using Scenes.GameScene;
using Scenes.GameScene.Bottle;
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
            Container.Bind<ISaveSystem<PlayerData>>()
                .To<SaveSystemAdapter<PlayerData>>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<LevelController>().AsSingle().WithArguments(hintManager);
            Container.Bind<IMoveStrategy>()
                .To<BfsMoveStrategy>()
                .AsSingle();
            Container.Bind<BottlesController>().AsSingle();
            Container.Bind<MovesManager>().AsSingle();
            Container.Bind<BottlesContainer>().AsSingle().WithArguments(layoutSettings, bottlesParentTransform);
            
            Container.Bind<IBottleFactory>()
                .To<UnityBottleFactory>()
                .AsSingle()
                .WithArguments(bottlePrefab);

            Debug.Log("Game scene data loaded");
        }
    }
}
using Common.DataManagement;
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
        [SerializeField] private BottlesContainer bottlesContainer;
        [SerializeField] private LevelController levelController;
        [SerializeField] private ColorPaletteController colorPaletteController;
        [SerializeField] private HintManager hintManager;

        [ContextMenu("Clear player data")]
        private void ClearPlayerData()
        {
            SaveSystem<PlayerData>.Instance.Clear();
        }

        public override void InstallBindings()
        {
            colorPaletteController.LoadPalette(0);
            levelController.Initialize(colorPaletteController.GetColorPalette(), hintManager);
            levelController.LoadLevel();
            Debug.Log("Game scene data loaded");
        }
    }
}
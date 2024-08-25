using Scenes.GameScene.Bottle;
using Scenes.GameScene.ColorPalette;
using Scenes.GameScene.Level;
using UnityEngine;

namespace Scenes.GameScene
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] private BottlesContainer bottlesContainer;
        [SerializeField] private LevelController levelController;
        [SerializeField] private ColorPaletteController colorPaletteController;
        
        private void Start()
        {
            colorPaletteController.LoadPalette(0);
            levelController.Initialize(colorPaletteController.GetColorPalette());
            levelController.LoadLevel();
            Debug.Log("Game scene data loaded");
        }
    }
}

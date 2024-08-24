using UnityEngine;

namespace Scenes.GameScene.ColorPalette
{
    public class ColorPaletteController : MonoBehaviour
    {
        [SerializeField] private ColorPaletteCollection paletteCollection;
        public ColorPalette currentColorPalette;

        public void LoadPalette(int paletteIndex)
        {
            if (paletteIndex >= 0 && paletteIndex < paletteCollection.colorPalettes.Count)
            {
                currentColorPalette = paletteCollection.colorPalettes[paletteIndex];
            }
            else Debug.Log("Color palette not found");
        }

        public ColorPalette GetColorPalette()
        {
            return currentColorPalette;
        }
    }
}

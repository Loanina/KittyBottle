using System;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.ColorPalette
{
    public class ColorPaletteController : IInitializable
    {
        private readonly ColorPaletteCollection colorPaletteCollection;
        private readonly int selectedPaletteIndex;
        private ColorPalette currentColorPalette;

        [Inject]
        public ColorPaletteController(ColorPaletteCollection colorPaletteCollection, int paletteIndex)
        {
            this.colorPaletteCollection = colorPaletteCollection;
            selectedPaletteIndex = paletteIndex;
        }

        public void Initialize()
        {
            SelectPalette(selectedPaletteIndex);
        }

        private void SelectPalette(int paletteIndex)
        {
            if (paletteIndex < 0 || paletteIndex >= colorPaletteCollection.colorPalettes.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(paletteIndex), "Invalid palette index");
            }
            currentColorPalette = colorPaletteCollection.colorPalettes[paletteIndex];
        }

        public ColorPalette GetCurrentColorPalette() => currentColorPalette;
        
        public Color GetColorByIndex(int index) => currentColorPalette.GetColor(index);
    }
}

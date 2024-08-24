using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.ColorPalette
{
    [CreateAssetMenu(fileName = "ColorPaletteCollection", menuName = "Game/Color palette collection")]
    public class ColorPaletteCollection : ScriptableObject
    {
        public List<ColorPalette> colorPalettes;
    }
}

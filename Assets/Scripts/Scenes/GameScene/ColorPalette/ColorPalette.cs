using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.ColorPalette
{
    [Serializable]
    public class ColorPalette
    {
        public List<Color> colors;

        public Color GetColor(int colorIndex)
        {
            if (colorIndex >= 0 && colorIndex < colors.Count)
            {
                return colors[colorIndex];
            }
            throw new IndexOutOfRangeException("Color index out of the range");
        }
    }
}

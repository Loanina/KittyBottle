using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.Bottle.Shader
{
    public class ColorStackHandler
    {
        private readonly int maxColors;
        public Stack<Color> Colors { get; private set; } = new();
        public Color TopColor { get; private set; }
        public int TopColorCount { get; private set; }

        public ColorStackHandler(int maxColors)
        {
            this.maxColors = maxColors;
        }

        public void Initialize(List<Color> initialColors)
        {
            Colors = new Stack<Color>(initialColors);
            UpdateTopColor();
        }

        public void Add(Color color, int count)
        {
            for (var i = 0; i < count; i++)
                Colors.Push(color);

            UpdateTopColor();
        }

        public void Remove(int count)
        {
            for (var i = 0; i < count && Colors.Count > 0; i++)
                Colors.Pop();

            UpdateTopColor();
        }

        public bool CanAdd(Color color) =>
            Colors.Count == 0 || (color.Equals(TopColor) && Colors.Count < maxColors);

        public bool IsFullByOneColor() => TopColorCount == maxColors;
        public bool IsEmpty() => Colors.Count == 0;

        private void UpdateTopColor()
        {
            if (Colors.Count == 0)
            {
                TopColor = default;
                TopColorCount = 0;
                return;
            }

            var colorsCopy = Colors.ToArray();
            TopColor = colorsCopy[0];
            TopColorCount = 1;

            for (int i = 1; i < colorsCopy.Length; i++)
            {
                if (!colorsCopy[i].Equals(TopColor))
                    break;

                TopColorCount++;
            }
        }

        public int EmptySpace()
        {
            return maxColors - Colors.Count;
        }
    }
}
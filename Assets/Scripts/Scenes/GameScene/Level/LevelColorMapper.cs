using System.Collections.Generic;
using System.Linq;
using Scenes.GameScene.ColorPalette;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Level
{
    public class LevelColorMapper
    {
        private readonly ColorPaletteController colorPaletteController;

        [Inject]
        public LevelColorMapper(ColorPaletteController colorPaletteController)
        {
            this.colorPaletteController = colorPaletteController;
        }

        public List<List<Color>> MapLevelDataToColors(LevelData levelData)
        {
            var colors = new List<List<Color>>();
            foreach (var bottle in levelData.level)
            {
                var bottleColors = bottle.Select(index => 
                    colorPaletteController.GetColorByIndex(index)
                ).ToList();
                colors.Add(bottleColors);
            }
            return colors;
        }
    }
}
using System;
using System.Collections.Generic;
using Scenes.GameScene.Bottle;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelCollection levelCollection;
        private LevelData currentLevel;
        
        public void LoadLevel(int levelIndex, BottlesContainer bottlesContainer, ColorPalette.ColorPalette colorPalette)
        {
            if (levelIndex >= 0 && levelIndex < levelCollection.levels.Count)
            {
                currentLevel = levelCollection.levels[levelIndex];
                bottlesContainer.CreateBottles(GetLevelColorsFromPalette(colorPalette));
            }
            else Debug.Log("Level not find");
        }

        private List<List<Color>> GetLevelColorsFromPalette(ColorPalette.ColorPalette colorPalette)
        {
            try
            {
                var levelColors = new List<List<Color>>();
                foreach (var bottle in currentLevel.level)
                {
                    var newBottle = new List<Color>();
                    foreach (var colorIndex in bottle)
                    {
                        newBottle.Add(colorPalette.GetColor(colorIndex));
                    }
                    levelColors.Add(newBottle);
                }
                return levelColors;
            }
            catch(Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
    }
}

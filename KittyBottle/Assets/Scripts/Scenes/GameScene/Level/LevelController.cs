using System;
using System.Collections.Generic;
using Scenes.GameScene.Bottle;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelCollection levelCollection;
        [SerializeField] private BottlesContainer bottlesContainer;
        [SerializeField] private ColorPalette.ColorPalette colorPalette;
        private LevelData currentLevel;
        [SerializeField] private int currentLevelIndex = 0;
        
        public void Initialize(ColorPalette.ColorPalette palette)
        {
            colorPalette = palette;
            bottlesContainer.OnLevelComplete += OnLevelComplete;
        }
        
        public void LoadLevel()
        {
            if (currentLevelIndex >= 0 && currentLevelIndex < levelCollection.levels.Count)
            {
                currentLevel = levelCollection.levels[currentLevelIndex];
                bottlesContainer.CreateBottles(GetLevelColorsFromPalette());
            }
            else Debug.Log("Level not find");
        }

        private List<List<Color>> GetLevelColorsFromPalette()
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

        private void OnLevelComplete()
        {
            bottlesContainer.DeleteBottles();
            currentLevelIndex += 1;
            LoadLevel();
        }
    }
}

using System;
using System.Collections.Generic;
using Common.DataManagement;
using Scenes.GameScene.Bottle;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelCollection levelCollection;
        [SerializeField] private BottlesContainer bottlesContainer;
        [SerializeField] private ColorPalette.ColorPalette colorPalette;
        [SerializeField] private int currentLevelIndex = 0;
        private LevelData currentLevel;
        
        public void Initialize(ColorPalette.ColorPalette palette, HintManager hintManager)
        {
            colorPalette = palette;
            currentLevelIndex = SaveSystem<PlayerData>.Instance.Load().lastLevelID + 1;
            bottlesContainer.OnLevelComplete += OnLevelComplete;
            hintManager.OnGetBestMoveEvent += bottlesContainer.OnBestMove;
            hintManager.OnRestartEvent += OnRestartLevel;
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
            SavePlayerData();
            bottlesContainer.DeleteBottles();
            currentLevelIndex += 1;
            LoadLevel();
        }

        private void OnRestartLevel()
        {
            bottlesContainer.DeleteBottles();
            LoadLevel();
        }

        private void SavePlayerData()
        {
            var data = SaveSystem<PlayerData>.Instance.Load();
            data.coins += 10;
            data.lastLevelID = currentLevelIndex;
            SaveSystem<PlayerData>.Instance.Save(data);
            Debug.Log($"Player data saved: last level ID: {data.lastLevelID}, coins: {data.coins}");
        }
    }
}

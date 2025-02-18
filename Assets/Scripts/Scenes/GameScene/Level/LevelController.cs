using System;
using Core.SavingSystem;
using Scenes.GameScene.Bottle;
using Zenject;

namespace Scenes.GameScene.Level
{
    public class LevelController : IInitializable, IDisposable
    {
        private int currentLevelIndex;
    
        private readonly LevelProvider levelProvider;
        private readonly BottlesContainer bottlesContainer;
        private readonly LevelColorMapper colorMapper;
        private readonly PlayerProgressService progressService;
        private readonly HintManager hintManager;

        [Inject]
        public LevelController(
            LevelProvider levelProvider,
            BottlesContainer bottlesContainer,
            LevelColorMapper colorMapper,
            PlayerProgressService progressService,
            HintManager hintManager)
        {
            this.levelProvider = levelProvider;
            this.bottlesContainer = bottlesContainer;
            this.colorMapper = colorMapper;
            this.progressService = progressService;
            this.hintManager = hintManager;
        }

        public void Initialize()
        {
            currentLevelIndex = LoadProgress();
            SubscribeToEvents();
            LoadLevel();
        }
    

        private int LoadProgress() => 
            progressService.GetLastCompletedLevel() + 1;

        private void SubscribeToEvents()
        {
            bottlesContainer.OnLevelComplete += HandleLevelComplete;
            hintManager.OnBestMoveRequested += bottlesContainer.CalculateBestMove;
            hintManager.OnRestartRequested += OnRestartLevel;
        }

        private void LoadLevel()
        {
            if (!levelProvider.HasLevel(currentLevelIndex))
            {
                throw new ArgumentOutOfRangeException(nameof(currentLevelIndex));
            }

            var levelData = levelProvider.GetLevel(currentLevelIndex);
            var colors = colorMapper.MapLevelDataToColors(levelData);
            bottlesContainer.CreateBottles(colors);
        }

        private void HandleLevelComplete()
        {
            progressService.UpdateProgress(currentLevelIndex);
            bottlesContainer.DeleteBottles();
            currentLevelIndex++;
            LoadLevel();
        }

        private void OnRestartLevel()
        {
            bottlesContainer.DeleteBottles();
            LoadLevel();
        }

        public void Dispose()
        {
            bottlesContainer.OnLevelComplete -= HandleLevelComplete;
            hintManager.OnBestMoveRequested -= bottlesContainer.CalculateBestMove;
            hintManager.OnRestartRequested -= OnRestartLevel;
        }
    }
}
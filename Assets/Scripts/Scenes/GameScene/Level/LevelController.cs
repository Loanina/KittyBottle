using System;
using Core.SavingSystem;
using Scenes.GameScene.Bottle;
using Scenes.GameScene.Hints;
using Scenes.GameScene.Reward;
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
        private readonly RewardService rewardService;

        [Inject]
        public LevelController(
            LevelProvider levelProvider,
            BottlesContainer bottlesContainer,
            LevelColorMapper colorMapper,
            PlayerProgressService progressService,
            HintManager hintManager,
            RewardService rewardService)
        {
            this.levelProvider = levelProvider;
            this.bottlesContainer = bottlesContainer;
            this.colorMapper = colorMapper;
            this.progressService = progressService;
            this.hintManager = hintManager;
            this.rewardService = rewardService;
        }

        public void Initialize()
        {
            currentLevelIndex = LoadProgress();
            SubscribeToEvents();
            LoadLevel(currentLevelIndex);
        }
        
        private int LoadProgress() => 
            progressService.GetLastCompletedLevelID() + 1;

        private void SubscribeToEvents()
        {
            hintManager.OnRestartRequested += OnRestartLevel;
        }

        private void LoadLevel(int index)
        {
            if (!levelProvider.HasLevel(index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var levelData = levelProvider.GetLevel(index);
            var colors = colorMapper.MapLevelDataToColors(levelData);
            bottlesContainer.CreateBottles(colors);
        }

        public void HandleLevelComplete()
        {
            bottlesContainer.DeleteBottles();
            rewardService.ShowRewards(levelProvider.GetLevel(currentLevelIndex).reward, () =>
            {
                progressService.SetLastCompletedLevel(currentLevelIndex);
                currentLevelIndex++;
                LoadLevel(currentLevelIndex);
            });
        }

        private void OnRestartLevel()
        {
            bottlesContainer.DeleteBottles();
            LoadLevel(currentLevelIndex);
        }

        public void Dispose()
        {
            hintManager.OnRestartRequested -= OnRestartLevel;
        }
    }
}
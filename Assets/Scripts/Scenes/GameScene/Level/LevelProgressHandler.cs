using System;
using Scenes.GameScene.Bottle;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Level
{
    public class LevelProgressHandler : IInitializable, IDisposable
    {
        private readonly BottlesController bottlesController;
        private readonly LevelCompletionChecker completionChecker;
        private readonly LevelController levelController;
        private readonly BottlesContainer bottlesContainer;

        [Inject]
        public LevelProgressHandler(
            BottlesController bottlesController,
            LevelCompletionChecker completionChecker,
            LevelController levelController,
            BottlesContainer bottlesContainer)
        {
            this.bottlesController = bottlesController;
            this.completionChecker = completionChecker;
            this.levelController = levelController;
            this.bottlesContainer = bottlesContainer;
        }

        public void Initialize()
        {
            bottlesController.OnPouringEnd += OnPouringEnd;
        }

        private void OnPouringEnd()
        {
            var bottles = bottlesContainer.GetAllBottles();
            if (completionChecker.CheckIfLevelComplete(bottles))
            {
                Debug.Log("Level completed!");
                levelController.HandleLevelComplete();
            }
        }

        public void Dispose()
        {
            bottlesController.OnPouringEnd -= OnPouringEnd;
        }
    }
}
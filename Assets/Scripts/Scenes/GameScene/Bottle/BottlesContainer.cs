using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainer
    {
        private readonly IBottleFactory bottleFactory;
        private readonly LayoutSettings layoutSettings;
        private readonly Transform parent;
        private readonly IGameLogger logger;
        
        private List<Bottle> bottles;
        public event Action<List<Bottle>> OnBottlesCreated;
        public event Action OnBottlesDeleted;
        public event Action OnLevelComplete;

        [Inject]
        public BottlesContainer(
            LayoutSettings layoutSettings,
            Transform parent,
            IBottleFactory bottleFactory,
            IGameLogger logger)
        {
            this.layoutSettings = layoutSettings;
            this.parent = parent;
            this.bottleFactory = bottleFactory;
            this.logger = logger;
        }

        public void CreateBottles(List<List<Color>> colors)
        {
            bottles = new List<Bottle>();
            var layout = layoutSettings.GetLayout(colors.Count);
            
            for (var i = 0; i < colors.Count; i++)
            {
                var position = parent.TransformPoint(layout[i]);
                var bottle = bottleFactory.CreateBottle(position, parent);
                bottle.Initialize(colors[i]);
                bottle.OnPouringEnd += CheckLevelCompletion;
                bottles.Add(bottle);
            }
            OnBottlesCreated?.Invoke(bottles);
            Debug.Log("OnBottlesCreated");
        }

        public void DeleteBottles()
        {
            if (bottles == null) return;

            foreach (var bottle in bottles)
            {
                bottle.OnPouringEnd -= CheckLevelCompletion;
                bottleFactory.DestroyBottle(bottle);
            }
            bottles.Clear();
            OnBottlesDeleted?.Invoke();
        }

        private void CheckLevelCompletion()
        {
            const int MAX_LAYERS = 4;
            bool isCompleted = bottles.All(b => 
                b.GetNumberOfTopColorLayers() == MAX_LAYERS || b.IsEmpty());

            if (isCompleted)
            {
                logger.Log("Level completed!");
                OnLevelComplete?.Invoke();
            }
        }

        public Bottle GetBottle(int index) => 
            bottles[index];

        public int GetIndexOfBottle(Bottle bottle)
        {
            if (bottles == null)
            {
                logger.LogError("Bottles list is not initialized!");
                return -1;
            }

            if (bottle == null)
            {
                logger.LogError("Bottle reference is null!");
                return -1;
            }

            return bottles.IndexOf(bottle);
        }

        public List<Bottle> GetAllBottles() => bottles;
    }
}
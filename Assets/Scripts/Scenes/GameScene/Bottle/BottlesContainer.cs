using System;
using System.Collections.Generic;
using System.Linq;
using Core.Hints;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainer
    {
        private readonly IBottleFactory bottleFactory;
        private LayoutSettings layoutSettings;
        private Transform parent;
        
        private IMoveStrategy moveStrategy;
        private BottlesController bottlesController;
        private ILogger logger;
        private List<Bottle> bottles;
        public event Action OnLevelComplete;

        public BottlesContainer (LayoutSettings layoutSettings, Transform parent,
            IMoveStrategy moveStrategy, BottlesController bottlesController, ILogger logger, IBottleFactory bottleFactory)
        {
            this.bottleFactory = bottleFactory;
            this.layoutSettings = layoutSettings;
            this.parent = parent;
            this.moveStrategy = moveStrategy;
            this.bottlesController = bottlesController;
            this.logger = logger;
        }

        public void CreateBottles(List<List<Color>> colors) 
        {
            bottles = new List<Bottle>();
            var layout = layoutSettings.GetLayout(colors.Count);
            for (int i = 0; i < colors.Count; i++)
            {
                var bottle = bottleFactory.CreateBottle( parent.TransformPoint(layout[i]), parent);
                bottle.Initialize(colors[i]);
                bottle.OnClickEvent += (clickedBottle) => bottlesController.PeekBottle(clickedBottle);
                bottle.OnPouringEnd += CheckCompletion;
                bottles.Add(bottle);
            }
        }
        
        public void DeleteBottles()
        {
            foreach (var bottle in bottles)
            {
                DOTween.Kill(bottle.gameObject);
                bottleFactory.DestroyBottle(bottle);
            }
            bottles.Clear();
        }

        private void CheckCompletion()
        {
            const int MAX_LAYERS = 4;
            bool isCompleted = bottles.All(b => 
                b.GetNumberOfTopColorLayers() == MAX_LAYERS || b.IsEmpty());

            if (isCompleted) OnLevelComplete?.Invoke();
        }

        public void CalculateBestMove()
        {
            var bottlesSnapshot = bottles.Select(b => new BottleState(b)).ToList();
            var move = moveStrategy.FindBestMove(bottlesSnapshot);
            logger.Log($"Best move: {move.Item1} -> {move.Item2}");
        }

        public Bottle GetBottle(int index)
        {
            return bottles[index];
        }
        
        public int GetIndexOfBottle(Bottle bottle)
        {
            if (bottles == null)
            {
                Debug.LogError("Bottles list is not initialized!");
                return -1;
            }
            if (bottle != null) return bottles.IndexOf(bottle);
            Debug.LogError("Bottle reference is null!");
            return -1;

        }
    }
}
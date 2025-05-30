using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainer
    {
        private readonly BottleFactory bottleFactory;
        private readonly LayoutSettings layoutSettings;

        private List<Bottle> bottles;
        public event Action<List<Bottle>> OnBottlesCreated;
        public event Action<List<Bottle>> OnBottlesDeleted;

        [Inject]
        public BottlesContainer(
            LayoutSettings layoutSettings,
            BottleFactory bottleFactory)
        {
            this.layoutSettings = layoutSettings;
            this.bottleFactory = bottleFactory;
        }

        public void CreateBottles(List<List<Color>> colors)
        {
            bottles = new List<Bottle>();
            var layout = layoutSettings.GetLayout(colors.Count);

            for (var i = 0; i < colors.Count; i++)
            {
                var bottle = bottleFactory.Create();
                bottle.transform.localPosition = layout[i];
                bottle.Initialize(colors[i]);
                bottles.Add(bottle);
            }
            OnBottlesCreated?.Invoke(bottles);
        }

        public void DeleteBottles()
        {
            if (bottles == null) return;
            OnBottlesDeleted?.Invoke(bottles);

            foreach (var bottle in bottles)
            {
                bottle.CancelAnimationAsync()
                    .ContinueWith(() => UnityEngine.Object.Destroy(bottle.gameObject)).Forget();
            }
            bottles.Clear();
        }

        public Bottle GetBottle(int index) => 
            bottles[index];

        public int GetIndexOfBottle(Bottle bottle)
        {
            if (bottle == null) throw new ArgumentNullException(nameof(bottle));
            if (bottles == null) throw new ArgumentNullException(nameof(bottles));
            return bottles.IndexOf(bottle);
        }

        public List<Bottle> GetAllBottles() => bottles;
    }
}

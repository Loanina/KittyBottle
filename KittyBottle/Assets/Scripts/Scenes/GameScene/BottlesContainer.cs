using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.GameScene
{
    public class BottlesContainer : SerializedMonoBehaviour
    {
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private List<List<Color>> levelColors;
        [SerializeField] private BottlesController bottlesController;
        private List<Bottle> bottles;

        private void CreateBottles()
        {
            bottles = new List<Bottle>();
            foreach (var bottleColors in levelColors)
            {
                var bottle = Instantiate(bottlePrefab, transform);
                bottle.Initialize(bottleColors);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
            }
        }

        private void Start()
        {
            CreateBottles();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            bottles.ForEach(bottle => bottle.SetDefaultPosition());
        }
    }
}

using System;
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
        [SerializeField] private Dictionary<int, List<Vector3>> layoutSettings;
        private List<Bottle> bottles;
        
        private void CreateBottles()
        {
            /*
            bottles = new List<Bottle>();
            foreach (var bottleColors in levelColors)
            {
                var bottle = Instantiate(bottlePrefab, transform);
                bottle.Initialize(bottleColors);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
            }
            */
            bottles = new List<Bottle>();
            for (var i = 0; i < levelColors.Count; i++)
            {
                var position = transform.TransformPoint(layoutSettings[levelColors.Count][i]);
                var bottle = Instantiate(bottlePrefab, position, new Quaternion(), transform);
                bottle.Initialize(levelColors[i]);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
                bottle.SetDefaultPosition();
            }
        }

        private void Start()
        {
            CreateBottles();
            /*
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            bottles.ForEach(bottle => bottle.SetDefaultPosition());
            */  
        }

        [Serializable]
        private class LayoutParams
        {
            public Vector3 position;
            public Vector3 scale;
        }
    }
}

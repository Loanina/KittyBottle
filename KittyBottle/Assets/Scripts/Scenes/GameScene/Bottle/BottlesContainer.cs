using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.GameScene.Bottle
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
            bottles = new List<Bottle>();
            for (var i = 0; i < levelColors.Count; i++)
            {
                var position = transform.TransformPoint(layoutSettings[levelColors.Count][i]);
                var bottle = Instantiate(bottlePrefab, position, new Quaternion(), transform);
                bottle.Initialize(levelColors[i]);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
                bottle.OnEndPouring += CheckLevelCompletion;
                bottle.SetDefaultPosition();
            }
        }

        private void Start()
        {
            CreateBottles();
        }

        private void CheckLevelCompletion()
        {
            foreach (var bottle in bottles)
            {
                var numberOfTopColorLayers = bottle.GetNumberOfTopColorLayers();
                if (numberOfTopColorLayers !=4 & numberOfTopColorLayers !=0) return;
            }
            Debug.Log("LEVEL COMPLETE!!!!!!!");
        }

        [Serializable]
        private class LayoutParams
        {
            public Vector3 position;
            public Vector3 scale;
        }
    }
}

using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainer : SerializedMonoBehaviour
    {
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private BottlesController bottlesController;
        [SerializeField] private LayoutSettings layoutSettings;
        private List<Bottle> bottles;
        public event Action OnLevelComplete;

        public void CreateBottles(List<List<Color>> levelColors)
        {
            bottles = new List<Bottle>();
            var layout = layoutSettings.GetLayout(levelColors.Count);
            for (var i = 0; i < levelColors.Count; i++)
            {
                var position = transform.TransformPoint(layout[i]);
                var bottle = Instantiate(bottlePrefab, position, new Quaternion(), transform);
                bottle.Initialize(levelColors[i]);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
                bottle.OnEndPouring += CheckLevelCompletion;
                bottle.SetDefaultPosition();
            }
        }

        public void DeleteBottles()
        {
            if (bottles == null) return;
            foreach (var bottle in bottles)
            {
                DOTween.Kill(bottle.gameObject);
                bottle.OnClickEvent -= bottlesController.OnClickBottle;
                bottle.OnEndPouring -= CheckLevelCompletion;
                Destroy(bottle.gameObject);
            }
            bottles.Clear();
            Debug.Log("Bottles was deleted");
        }

        private void CheckLevelCompletion()
        {
            foreach (var bottle in bottles)
            {
                var numberOfTopColorLayers = bottle.GetNumberOfTopColorLayers();
                if (numberOfTopColorLayers !=4 & numberOfTopColorLayers !=0) return;
            }
            Debug.Log("LEVEL COMPLETE!!!!!!!");
            OnLevelComplete?.Invoke();
        }
    }
}

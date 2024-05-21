using System.Collections.Generic;
using Scenes.GameScene;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

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
    }
}

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BottlesContainer : SerializedMonoBehaviour
{
    [SerializeField] private Bottle bottlePrefab;
    [SerializeField] private List<List<Color>> levelColors;
    private List<Bottle> bottles;

    private void CreateBottles()
    {
        bottles = new List<Bottle>();
        foreach (var bottleColors in levelColors)
        {
            var bottle = Instantiate(bottlePrefab, transform);
            bottle.Initialize(bottleColors);
            bottles.Add(bottle);
        }
    }

    private void Start()
    {
        CreateBottles();
    }
}

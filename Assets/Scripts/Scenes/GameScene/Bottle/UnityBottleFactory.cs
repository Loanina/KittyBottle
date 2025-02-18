using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class UnityBottleFactory : IBottleFactory
    {
        private readonly Bottle bottlePrefab;

        [Inject]
        public UnityBottleFactory(Bottle bottlePrefab)
        {
            this.bottlePrefab = bottlePrefab;
        }

        public Bottle CreateBottle(Vector3 position, Transform parent)
        {
            return Object.Instantiate(bottlePrefab, position, Quaternion.identity, parent);
        }

        public void DestroyBottle(Bottle bottle)
        {
            if (bottle != null)
                Object.Destroy(bottle.gameObject);
        }
    }
}
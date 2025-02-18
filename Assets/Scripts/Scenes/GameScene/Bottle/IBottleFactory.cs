using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public interface IBottleFactory
    {
        Bottle CreateBottle(Vector3 position, Transform parent);
        void DestroyBottle(Bottle bottle);
    }
}
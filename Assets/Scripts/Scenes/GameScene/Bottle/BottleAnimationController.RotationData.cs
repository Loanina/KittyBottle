using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public partial class BottleAnimationController
    {
        private struct RotationData
        {
            public readonly Transform RotationPoint;
            public readonly Vector3 PouringOffset;
            public readonly bool IsClockwise;

            public RotationData(Transform rotationPoint, Vector3 pouringOffset, bool isClockwise)
            {
                RotationPoint = rotationPoint;
                PouringOffset = pouringOffset;
                IsClockwise = isClockwise;
            }
        }
    }
}
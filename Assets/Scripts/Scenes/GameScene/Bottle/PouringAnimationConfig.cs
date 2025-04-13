using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    [CreateAssetMenu(fileName = "PouringAnimationConfig", menuName = "Game/Animations/Pouring animation settings")]
    public class PouringAnimationConfig : ScriptableObject
    {
        public float[] rotationValues = {33.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [Range(0,10)] public float timeRotation = 0.7f;
        [Range(0,10)] public float timeMove = 0.5f;
        public AnimationCurve rotationSpeedMultiplier;
    }
}
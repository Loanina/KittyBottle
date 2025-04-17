using UnityEngine;

namespace Scenes.GameScene.Bottle.Animation
{
    [CreateAssetMenu(fileName = "BottleAnimationConfig", menuName = "Game/Animations/Bottle Animation Config")]
    public class BottleAnimationConfig : ScriptableObject
    {
        [Header("Movement")]
        [Range(0,10)] public float timeMove = 0.5f;
        [Range(0.05f, 1f)] public float upOffset = 0.2f;

        [Header("Positions")]
        [Range(0f, 1f)] public float edgeThreshold = 0.31f;
        
        [Header("Pouring")]
        public float[] rotationValues = {33.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [Range(0,10)] public float timeRotation = 0.7f;
        public AnimationCurve rotationSpeedMultiplier;
    }
}
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    [CreateAssetMenu(fileName = "BottleShaderConfig", menuName = "Game/Bottle shader settings")]
    public class BottleShaderConfig : ScriptableObject
    { 
        public AnimationCurve scaleAndRotationMultiplierCurve;
        public AnimationCurve fillAmountCurve;
        public float[] fillAmountValues;
        [Range(1,20)] public int maxColorsInBottle = 4;
    }
}
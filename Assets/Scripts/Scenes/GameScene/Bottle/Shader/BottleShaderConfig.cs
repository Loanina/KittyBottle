using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.GameScene.Bottle.Shader
{
    [CreateAssetMenu(fileName = "BottleShaderConfig", menuName = "Game/Bottle shader settings")]
    public class BottleShaderConfig : ScriptableObject
    { 
        public AnimationCurve scaleAndRotationMultiplierCurve;
        public AnimationCurve fillAmountCurve;
        public float[] fillAmountValues;
        [InfoBox("To correctly change the maximum number of layers in a bottle, you need to change the shader"), 
         Range(1,20)] public int maxColorsInBottle = 4;
    }
}
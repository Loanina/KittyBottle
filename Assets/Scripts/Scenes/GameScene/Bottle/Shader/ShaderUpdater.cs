using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.Bottle.Shader
{
    public class ShaderUpdater
    {
        private readonly Material material;
        private readonly BottleShaderConfig config;
        private static readonly int ScaleRotProp = UnityEngine.Shader.PropertyToID("_ScaleAndRotationMultiplyProperty");
        private static readonly int FillAmountProp = UnityEngine.Shader.PropertyToID("_FillAmount");

        public ShaderUpdater(BottleShaderConfig config, Material material)
        {
            this.config = config;
            this.material = material;
        }

        public void UpdateFillAmount(int colorCount) =>
            material.SetFloat(FillAmountProp, config.fillAmountValues[colorCount]);

        public void SetRotation(float angle) =>
            material.SetFloat(ScaleRotProp, config.scaleAndRotationMultiplierCurve.Evaluate(angle));

        public void SetFillFromCurve(float angle) =>
            material.SetFloat(FillAmountProp, config.fillAmountCurve.Evaluate(angle));

        public void FillUp(float amount)
        {
            var current = material.GetFloat(FillAmountProp);
            material.SetFloat(FillAmountProp, current + amount);
        }

        public void SetColors(Stack<Color> colors)
        {
            var copy = new Stack<Color>(new Stack<Color>(colors));
            for (var i = copy.Count; i > 0; i--)
                material.SetColor($"_Color{i}", copy.Pop());
        }
        
        public float GetFillAmountFromArray(int count) =>
            config.fillAmountValues[count];

        public float GetFillAmountFromCurve(float angle) =>
            config.fillAmountCurve.Evaluate(angle);
    }
}
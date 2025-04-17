using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle.Shader
{
    public class BottleShaderController
    {
        private readonly BottleShaderConfig config;
        private readonly Material material;
        
        [Inject]
        public BottleShaderController(BottleShaderConfig config, BottleView view)
        {
            this.config = config;
            material = view.bottleMaskSR.material;
        }
        
        public Stack<Color> BottleColors { get; private set; }
        public Color TopColor { get; private set; }
        public int NumberOfTopColorLayers { get; private set; }
        
        private static readonly int ScaleAndRotationMultiplyProperty = UnityEngine.Shader.PropertyToID("_ScaleAndRotationMultiplyProperty");
        private static readonly int FillAmount = UnityEngine.Shader.PropertyToID("_FillAmount");
        
        private void UpdateTopColorValues()
        {
            if (BottleColors.Count == 0)
                NumberOfTopColorLayers = 0;
            else
            {
                NumberOfTopColorLayers = 1;
                var bottleColorsCopy = new Stack<Color>(new Stack<Color>(BottleColors));
                TopColor = bottleColorsCopy.Pop();

                while (bottleColorsCopy.TryPop(out var result) && result.Equals(TopColor))
                {
                    NumberOfTopColorLayers++;
                }
            }
        }

        private void UpdateColorsOnShader()
        {
            try
            {
                if (BottleColors.Count == 0) return;
                var bottleColorsCopy = new Stack<Color>(new Stack<Color>(BottleColors));
                for (var i = bottleColorsCopy.Count; i > 0; i--)
                {
                    material.SetColor("_Color" + i, bottleColorsCopy.Pop());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void RotateShaderComplete(float angleValue, int countOfColorToTransfer)
        {
            material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            material.SetFloat(FillAmount,config.fillAmountCurve.Evaluate(angleValue));
            for (var i = countOfColorToTransfer; i > 0; i--)
            {
                BottleColors.Pop();
            }
            UpdateTopColorValues();
        }

        public void HideTopColor(int count)
        {
            material.SetFloat(FillAmount, config.fillAmountValues[BottleColors.Count() - count]);
            for (var i = count; i > 0; i--)
            {
                BottleColors.Pop();
            }
            UpdateTopColorValues();
        }

        public void RotateShader(float angleValue, float lastAngleValue, Bottle targetBottle)
        {
            material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            
            if (config.fillAmountValues[BottleColors.Count] < config.fillAmountCurve.Evaluate(angleValue)) return;
            material.SetFloat(FillAmount,config.fillAmountCurve.Evaluate(angleValue));
            targetBottle.FillUp(config.fillAmountCurve.Evaluate(lastAngleValue) -
                                config.fillAmountCurve.Evaluate(angleValue));
        }

        public void RotateShader(float angleValue)
        {
            material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        public void Initialize(List<Color> bottleColors)
        {
            BottleColors = new Stack<Color>(bottleColors);
            material.SetFloat(FillAmount, config.fillAmountValues[BottleColors.Count]);
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }
        
        public void AddColor(int count, Color color)
        {
            for (var i = count; i > 0; i--)
            {
                BottleColors.Push(color);
            }
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }

        public bool EnableToFillBottle(Color color)
        {
            return BottleColors.Count==0 || (color.Equals(TopColor) && config.maxColorsInBottle > BottleColors.Count);
        }

        public int CalculateNumberOfColorsToTransfer(int countOfColor)
        {
            return  Mathf.Min(countOfColor, config.maxColorsInBottle - BottleColors.Count);
        }

        public int CalculateRotationIndexToAnotherBottle(int countOfColorToTransfer)
        {
            return config.maxColorsInBottle - BottleColors.Count + countOfColorToTransfer;
        }

        public int CalculateStartPouringIndex()
        {
            return config.maxColorsInBottle - BottleColors.Count;
        }

        public void FillUp(float fillAmountToAdd)
        {
            material.SetFloat(FillAmount,
                material.GetFloat(FillAmount) + fillAmountToAdd);
        }

        public bool IsEmpty()
        {
            return BottleColors.Count == 0;
        }

        public bool IsFullByOneColor()
        {
            return NumberOfTopColorLayers == config.maxColorsInBottle;
        }
    }
}

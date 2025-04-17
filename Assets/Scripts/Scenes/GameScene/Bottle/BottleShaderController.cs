using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottleShaderController
    {
        private readonly BottleShaderConfig config;
        private readonly SpriteRenderer mask;
        
        [Inject]
        public BottleShaderController(BottleShaderConfig config, BottleView view)
        {
            this.config = config;
            mask = view.bottleMaskSR;
        }
        
        public Stack<Color> _bottleColors { get; private set; }
        public Color TopColor { get; private set; }
        public int NumberOfTopColorLayers { get; private set; } = 1;
        private static readonly int ScaleAndRotationMultiplyProperty = Shader.PropertyToID("_ScaleAndRotationMultiplyProperty");
        private static readonly int FillAmount = Shader.PropertyToID("_FillAmount");
        
        private void UpdateTopColorValues()
        {
            if (_bottleColors.Count == 0)
                NumberOfTopColorLayers = 0;
            else
            {
                NumberOfTopColorLayers = 1;
                var bottleColorsCopy = new Stack<Color>(new Stack<Color>(_bottleColors));
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
                if (_bottleColors.Count == 0) return;
                var bottleColorsCopy = new Stack<Color>(new Stack<Color>(_bottleColors));
                for (var i = bottleColorsCopy.Count; i > 0; i--)
                {
                    mask.material.SetColor("_Color" + i, bottleColorsCopy.Pop());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void RotateShaderComplete(float angleValue, int countOfColorToTransfer)
        {
            mask.material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            mask.material.SetFloat(FillAmount,config.fillAmountCurve.Evaluate(angleValue));
            for (var i = countOfColorToTransfer; i > 0; i--)
            {
                _bottleColors.Pop();
            }
            UpdateTopColorValues();
        }

        public void HideTopColor(int count)
        {
            mask.material.SetFloat(FillAmount, config.fillAmountValues[_bottleColors.Count() - count]);
            for (var i = count; i > 0; i--)
            {
                _bottleColors.Pop();
            }
            UpdateTopColorValues();
        }

        public void RotateShader(float angleValue, float lastAngleValue, Bottle targetBottle)
        {
            mask.material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            
            if (config.fillAmountValues[_bottleColors.Count] < config.fillAmountCurve.Evaluate(angleValue)) return;
            mask.material.SetFloat(FillAmount,config.fillAmountCurve.Evaluate(angleValue));
            targetBottle.FillUp(config.fillAmountCurve.Evaluate(lastAngleValue) -
                                config.fillAmountCurve.Evaluate(angleValue));
        }

        public void RotateShader(float angleValue)
        {
            mask.material.SetFloat(ScaleAndRotationMultiplyProperty, config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }

        public void RotateShaderBack(float angleValue)
        {
            mask.material.SetFloat(ScaleAndRotationMultiplyProperty,
                config.scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        public void Initialize(List<Color> bottleColors)
        {
            _bottleColors = new Stack<Color>(bottleColors);
            mask.material.SetFloat(FillAmount, config.fillAmountValues[_bottleColors.Count]);
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }
        
        public void AddColor(int count, Color color)
        {
            for (var i = count; i > 0; i--)
            {
                _bottleColors.Push(color);
            }
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }

        public bool EnableToFillBottle(Color color)
        {
            return _bottleColors.Count==0 || (color.Equals(TopColor) && config.maxColorsInBottle > _bottleColors.Count);
        }

        public int CalculateNumberOfColorsToTransfer(int countOfColor)
        {
            return  Mathf.Min(countOfColor, config.maxColorsInBottle - _bottleColors.Count);
        }

        public int CalculateRotationIndexToAnotherBottle(int countOfColorToTransfer)
        {
            return config.maxColorsInBottle - _bottleColors.Count + countOfColorToTransfer;
        }

        public int CalculateStartPouringIndex()
        {
            return config.maxColorsInBottle - _bottleColors.Count;
        }

        public int GetCountOfColor()
        {
            return _bottleColors.Count;
        }

        public void FillUp(float fillAmountToAdd)
        {
            mask.material.SetFloat(FillAmount,
                mask.material.GetFloat(FillAmount) + fillAmountToAdd);
        }

        public bool IsEmpty()
        {
            return _bottleColors.Count == 0;
        }

        public bool IsFull()
        {
            return _bottleColors.Count == config.maxColorsInBottle;
        }

        public bool IsFullByOneColor()
        {
            return NumberOfTopColorLayers == config.maxColorsInBottle;
        }
    }
}

using System; 
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene
{
    public class BottleShaderController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bottleMaskSR;
        [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
        [SerializeField] private AnimationCurve fillAmountCurve;
        [SerializeField] private float[] fillAmountValues;
        [SerializeField] private int maxColorsInBottle = 4;
        
        private Stack<Color> _bottleColors;
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
                    bottleMaskSR.material.SetColor("_Color" + i, bottleColorsCopy.Pop());
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void RotateShaderComplete(float angleValue, int countOfColorToTransfer)
        {
            bottleMaskSR.material.SetFloat(ScaleAndRotationMultiplyProperty, scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat(FillAmount,fillAmountCurve.Evaluate(angleValue));
            for (var i = countOfColorToTransfer; i > 0; i--)
            {
                _bottleColors.Pop();
            }
            UpdateTopColorValues();
        }

        public void RotateShader(float angleValue, float lastAngleValue, Bottle targetBottle)
        {
            bottleMaskSR.material.SetFloat(ScaleAndRotationMultiplyProperty, scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            
            if (fillAmountValues[_bottleColors.Count] < fillAmountCurve.Evaluate(angleValue)) return;
            bottleMaskSR.material.SetFloat(FillAmount,fillAmountCurve.Evaluate(angleValue));
            targetBottle.FillUp(fillAmountCurve.Evaluate(lastAngleValue) -
                                fillAmountCurve.Evaluate(angleValue));
        }

        public void RotateShaderBack(float angleValue)
        {
            bottleMaskSR.material.SetFloat(ScaleAndRotationMultiplyProperty,
                scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        public void Initialize(List<Color> bottleColors)
        {
            _bottleColors = new Stack<Color>(bottleColors);
            bottleMaskSR.material.SetFloat(FillAmount, fillAmountValues[_bottleColors.Count]);
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
            return _bottleColors.Count==0 || (color.Equals(TopColor) && maxColorsInBottle > _bottleColors.Count);
        }

        public int CalculateNumberOfColorsToTransfer(int countOfColor)
        {
            return  Mathf.Min(countOfColor, maxColorsInBottle - _bottleColors.Count);
        }

        public int CalculateRotationIndexToAnotherBottle(int countOfColorToTransfer)
        {
            return maxColorsInBottle - _bottleColors.Count + countOfColorToTransfer;
        }

        public void FillUp(float fillAmountToAdd)
        {
            bottleMaskSR.material.SetFloat(FillAmount,
                bottleMaskSR.material.GetFloat(FillAmount) + fillAmountToAdd);
        }

        public bool IsBottleEmpty()
        {
            return _bottleColors.Count == 0;
        }
    }
}

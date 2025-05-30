using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.Bottle.Shader
{
    public class BottleShaderController
    {
        private readonly ColorStackHandler colorStackHandler;
        private readonly ShaderUpdater shader;

        public BottleShaderController(ColorStackHandler colorStackHandler, ShaderUpdater shader)
        {
            this.colorStackHandler = colorStackHandler;
            this.shader = shader;
        }

        public void Initialize(List<Color> colors)
        {
            colorStackHandler.Initialize(colors);
            shader.SetColors(colorStackHandler.Colors);
            UpdateFillAmount();
        }

        public void AddColor(Color color, int count, bool isVisible)
        {
            colorStackHandler.Add(color, count);
            shader.SetColors(colorStackHandler.Colors);
            if (isVisible) UpdateFillAmount();
        }

        public void RotateComplete(float angle)
        {
            shader.SetRotation(angle);
            shader.SetFillFromCurve(angle);
        }

        public void UpdateFillAmount()
        {
            shader.UpdateFillAmount(colorStackHandler.Colors.Count);
        }

        public void RemoveTopColor(int count)
        {
            colorStackHandler.Remove(count);
            UpdateFillAmount();
        }

        public void Rotate(float angle, float lastAngle, Bottle targetBottle)
        {
            shader.SetRotation(angle);

            if (colorStackHandler.Colors.Count == 0) return;

            var targetFill = shader.GetFillAmountFromCurve(angle);
            var currentFill = shader.GetFillAmountFromArray(colorStackHandler.Colors.Count);

            if (currentFill < targetFill) return;

            shader.SetFillFromCurve(angle);
            var lastFill = shader.GetFillAmountFromCurve(lastAngle);
            targetBottle.FillUp(lastFill - targetFill);
        }

        public void FillUp(float amount) => shader.FillUp(amount);

        public void Rotate(float angleValue) => shader.SetRotation(angleValue);
        
        public bool CanFill(Color color) => colorStackHandler.CanAdd(color);
        public bool IsEmpty() => colorStackHandler.IsEmpty();
        public bool IsFullByOneColor() => colorStackHandler.IsFullByOneColor();

        public int CalculateNumberOfColorsToTransfer(int count) =>
            Mathf.Min(count, colorStackHandler.EmptySpace());

        public int CalculateStartPouringIndex() =>
            colorStackHandler.EmptySpace();

        public int CalculateRotationIndex(int countToTransfer) =>
            CalculateStartPouringIndex() + countToTransfer;

        public Stack<Color> Colors() => colorStackHandler.Colors;
        public Color TopColor() => colorStackHandler.TopColor;
        public int NumberOfTopColor() => colorStackHandler.TopColorCount;
    }
}

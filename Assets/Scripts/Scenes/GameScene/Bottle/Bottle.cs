using System;
using System.Collections.Generic;
using System.Threading;
using Core.InputSystem;
using Cysharp.Threading.Tasks;
using Scenes.GameScene.Bottle.Animation;
using Scenes.GameScene.Bottle.Shader;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class Bottle : MonoBehaviour, IClickable
    {
        private BottleShaderController shaderController;
        private BottleAnimationController animationController;
        private BottleView view;
        private BottleUsageController usageController;
        private CancellationTokenSource pouringCts;
        private bool isPouring;
        public event Action<Bottle> OnClicked;

        [Inject]
        public void Construct(BottleShaderController shaderController, BottleAnimationController animationController, 
            BottleView view, BottleUsageController usageController)
        {
            this.shaderController = shaderController;
            this.animationController = animationController;
            this.view = view;
            this.usageController = usageController;
        }

        public void Initialize(List<Color> bottleColors)
        {
            shaderController.Initialize(bottleColors);
            animationController.SetDefaultPosition();
        }

        private void OnDestroy()
        {
            pouringCts?.Cancel();
            pouringCts?.Dispose();
        }

        public void OnClick()
        {
            OnClicked?.Invoke(this);
        }

        private CancellationToken GetPouringCancellationToken()
        {
            pouringCts?.Dispose();
            pouringCts = new CancellationTokenSource();
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                pouringCts.Token, this.GetCancellationTokenOnDestroy()
            ).Token;

            return linkedToken;
        }

        public async UniTask CancelAnimationAsync()
        {
            if (pouringCts == null || pouringCts.IsCancellationRequested)
                return;

            pouringCts.Cancel();

            await UniTask.WaitWhile(() => isPouring,
                cancellationToken: this.GetCancellationTokenOnDestroy());
        }
        
        public void FillUp(float value)
        {
            shaderController.FillUp(value);
        }

        public void AddColor(Color color, int count, bool visible)
        {
            shaderController.AddColor(color, count, visible);
        }

        public void RemoveTopColor(int count)
        {
            shaderController.RemoveTopColor(count);
        }

        public UniTask PouringAnimationAsync(Bottle targetBottle,
            Color colorToTransfer,
            int countOfColorToTransfer) => 
            animationController.PouringAnimationAsync(targetBottle, colorToTransfer, countOfColorToTransfer, GetPouringCancellationToken());

        public void GoToStartPosition() => animationController.GoToStartPosition();
        public void GoUp() => animationController.GoUp();

        public void UpdateFillAmount() => shaderController.UpdateFillAmount();

        public Color GetTopColor() => shaderController.TopColor();
        public Stack<Color> GetColors() => shaderController.Colors();
        public int GetTopColorLayers() => shaderController.NumberOfTopColor();
        public int GetTransferableCount(int count) => shaderController.CalculateNumberOfColorsToTransfer(count);

        public bool CanFill(Color color) => shaderController.CanFill(color);
        public bool IsEmpty() => shaderController.IsEmpty();
        public bool IsFullByOneColor() => shaderController.IsFullByOneColor();

        public void IncreaseUsageCount() => usageController.IncreaseUsageCount();
        public void DecreaseUsageCount() => usageController.DecreaseUsageCount();

        public void StartUse() => usageController.StartUse();
        public void EndUse() => usageController.EndUse();
        public bool InUse => usageController.InUse;
        public int UsesCount => usageController.UsesCount;
        public void SetSortingOrder(bool toUp) => view.SetSortingOrder(toUp);
        public void SetPouring(bool isPouring) => this.isPouring = isPouring;
    }
}
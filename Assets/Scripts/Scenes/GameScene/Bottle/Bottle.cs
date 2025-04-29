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
        private CancellationTokenSource pouringCts;

        public bool InUse { get; private set; }
        public int UsesCount { get; private set; }
        private bool IsPouring { get; set; }
        public event Action<Bottle> OnClicked;

        [Inject]
        public void Construct(BottleShaderController shaderController, BottleAnimationController animationController, BottleView view)
        {
            this.shaderController = shaderController;
            this.animationController = animationController;
            this.view = view;
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

        public async UniTask PourColorsAsync(Bottle target, Action onComplete = null)
        {
            var color = GetTopColor();
            var count = target.GetTransferableCount(GetTopColorLayers());

            target.AddColor(color, count, false);
            target.IncreaseUsageCount();

            InUse = true;
            view.SetSortingOrder(true);

            pouringCts?.Dispose();
            pouringCts = new CancellationTokenSource();
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                pouringCts.Token, this.GetCancellationTokenOnDestroy()
            ).Token;

            IsPouring = true;

            try
            {
                await animationController.PouringAnimationAsync(target, color, count, linkedToken);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Pouring was canceled");
            }
            finally
            {
                IsPouring = false;
                InUse = false;
                view.SetSortingOrder(false);
                pouringCts?.Dispose();
                pouringCts = null;
            }
        }

        public async UniTask CancelAnimationAsync()
        {
            if (pouringCts == null || pouringCts.IsCancellationRequested)
                return;

            pouringCts.Cancel();

            await UniTask.WaitWhile(() => IsPouring,
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

        private void IncreaseUsageCount() => UsesCount++;
        public void DecreaseUsageCount()
        {
            if (UsesCount > 0) UsesCount--;
        }
    }
}
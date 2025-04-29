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
            private BottleAnimationController bottleAnimationController;
            private BottleView view;
            private bool isPouring;
            public bool InUse { get; private set; }
            public int UsesCount { get; private set; }
            public event Action<Bottle> OnClicked;
            private CancellationTokenSource pouringCancellationTokenSource;

            [Inject]
            public void Construct(BottleShaderController shaderController, BottleAnimationController bottleAnimationController, BottleView view)
            {
                this.shaderController = shaderController;
                this.bottleAnimationController = bottleAnimationController;
                this.view = view;
            }
            
            public void Initialize(List<Color> bottleColors)
            {
                shaderController.Initialize(bottleColors);
                bottleAnimationController.SetDefaultPosition();
            }
            
            private void OnDestroy()
            {
                pouringCancellationTokenSource?.Cancel();
                pouringCancellationTokenSource?.Dispose();
            }
            
            public void OnClick() => OnClicked?.Invoke(this);
            
            public void FillUp(float fillUpToAdd) => shaderController.FillUp(fillUpToAdd);
            
            public void AddColor(Color color, int count, bool isVisible) => shaderController.AddColor(color, count, isVisible);

            public void RemoveTopColor(int count) => shaderController.RemoveTopColor(count);
            
            public async void PouringColorsBetweenBottles(Bottle targetBottle, Action onComplete = null)
            {
                var countOfColorToTransfer = targetBottle.NumberOfColorToTransfer(countOfColor: GetNumberOfTopColorLayers());
                var colorToTransfer = GetTopColor();
                targetBottle.AddColor(colorToTransfer, countOfColorToTransfer, false);
                targetBottle.IncreaseUsagesCount();
                InUse = true;
                view.SetSortingOrder(true);

                isPouring = true;
                pouringCancellationTokenSource?.Dispose();
                pouringCancellationTokenSource = new CancellationTokenSource();
                var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
                    pouringCancellationTokenSource.Token,
                    this.GetCancellationTokenOnDestroy()
                ).Token;

                try
                {
                    await bottleAnimationController.PouringAnimationAsync(
                        targetBottle,
                        colorToTransfer,
                        countOfColorToTransfer,
                        linkedToken
                    );
                    InUse = false;
                    view.SetSortingOrder(false);
                    onComplete?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    InUse = false;
                }
                finally
                {
                    isPouring = false;
                    pouringCancellationTokenSource?.Dispose();
                    pouringCancellationTokenSource = null;
                }
            }

            public async UniTask CancelAnimationAsync()
            {
                var cts = pouringCancellationTokenSource;
                if (cts == null || cts.IsCancellationRequested) 
                    return;
                cts.Cancel();
                await UniTask.WaitWhile(() => isPouring, 
                    cancellationToken: this.GetCancellationTokenOnDestroy());
            }

            public bool EnableToFill(Color color) => shaderController.CanFill(color);

            public Color GetTopColor() => shaderController.TopColor();

            public bool IsEmpty() => shaderController.IsEmpty();

            public bool IsFullByOneColor() => shaderController.IsFullByOneColor();

            public int GetNumberOfTopColorLayers() => shaderController.NumberOfTopColor();

            public int NumberOfColorToTransfer(int countOfColor) => shaderController.CalculateNumberOfColorsToTransfer(countOfColor);

            public Stack<Color> GetColors() => shaderController.Colors();

            private void IncreaseUsagesCount() => UsesCount += 1;

            public void DecreaseUsagesCount()
            {
                if (UsesCount > 0) UsesCount -= 1;
            }

            public void GoToStartPosition() => bottleAnimationController.GoToStartPosition();
            public void GoUp() => bottleAnimationController.GoUp();

            public void UpdateFillAmount() => shaderController.UpdateFillAmount();
        }
    }

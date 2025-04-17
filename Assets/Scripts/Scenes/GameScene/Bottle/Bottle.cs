    using System;
    using System.Collections.Generic;
    using Core.InputSystem;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Zenject;

    namespace Scenes.GameScene.Bottle
    {
        public class Bottle : MonoBehaviour, IClickable
        {
            private BottleShaderController shaderController;
            private BottleAnimationController bottleAnimationController;
            private BottleView view;
            public bool InUse { get; private set; }
            public int UsesCount { get; private set; }
            public event Action<Bottle> OnClicked;

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
            
            public void OnClick()
            {
                OnClicked?.Invoke(this);
            }
            
            public void FillUp(float fillUpToAdd)
            {
                shaderController.FillUp(fillUpToAdd);
            }

            public void AddColor(int count, Color color)
            {
                shaderController.AddColor(count, color);
            }

            public void RemoveTopColor(int count)
            {
                shaderController.HideTopColor(count);
            }
            
            public async void PouringColorsBetweenBottles(Bottle targetBottle, Action onComplete = null)
            {
                var countOfColorToTransfer = targetBottle.NumberOfColorToTransfer(GetNumberOfTopColorLayers());
                var colorToTransfer = GetTopColor();
                targetBottle.AddColor(countOfColorToTransfer, colorToTransfer);
                targetBottle.IncreaseUsagesCount();
                InUse = true;
                view.SetSortingOrder(true);

                var cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();

                try
                {
                    await bottleAnimationController.PouringAnimationAsync(
                        targetBottle,
                        colorToTransfer,
                        countOfColorToTransfer,
                        cancellationTokenOnDestroy
                    );
                    InUse = false;
                    view.SetSortingOrder(false);
                    onComplete?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    InUse = false;
                }
            }

            public bool EnableToFillBottle(Color color) => shaderController.EnableToFillBottle(color);

            public Color GetTopColor() => shaderController.TopColor;

            public bool IsEmpty() => shaderController.IsEmpty();

            public bool IsFull() => shaderController.IsFull();

            public bool IsFullByOneColor() => shaderController.IsFullByOneColor();

            private int GetNumberOfTopColorLayers() => shaderController.NumberOfTopColorLayers;

            private int NumberOfColorToTransfer(int countOfColor) => shaderController.CalculateNumberOfColorsToTransfer(countOfColor);

            public Stack<Color> GetColors() => shaderController._bottleColors;

            private void IncreaseUsagesCount() => UsesCount += 1;

            public void DecreaseUsagesCount()
            {
                if (UsesCount > 0) UsesCount -= 1;
            }

            public void GoToStartPosition() => bottleAnimationController.GoToStartPosition();
            public void GoUp() => bottleAnimationController.GoUp();
        }
    }

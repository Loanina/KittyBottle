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
                bottleAnimationController.ChooseRotationPointAndDirection(targetBottle.transform.position.x);
                InUse = true;
                SetSortingOrderUp();

                var ct = this.GetCancellationTokenOnDestroy();

                try
                {
                    await bottleAnimationController.PouringAnimationAsync(
                        targetBottle,
                        colorToTransfer,
                        countOfColorToTransfer,
                        shaderController,
                        ct
                    );
                    InUse = false;
                    SetSortingOrderDown();
                    onComplete?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    InUse = false;
                }
            }

            private void SetSortingOrderDown()
            {
                view.sortingGroup.sortingOrder = 1;
            }

            private void SetSortingOrderUp()
            {
                view.sortingGroup.sortingOrder = 2;
            }

            public bool EnableToFillBottle(Color color)
            {
                return shaderController.EnableToFillBottle(color);
            }

            public Color GetTopColor()
            {
                return shaderController.TopColor;
            }

            public bool IsEmpty()
            {
                return shaderController.IsEmpty();
            }

            public bool IsFull() => shaderController.IsFull();

            private int GetNumberOfTopColorLayers()
            {
                return shaderController.NumberOfTopColorLayers;
            }

            private int NumberOfColorToTransfer(int countOfColor)
            {
                return shaderController.CalculateNumberOfColorsToTransfer(countOfColor);
            }

            public Stack<Color> GetColors()
            {
                return shaderController._bottleColors;
            }

            private void IncreaseUsagesCount()
            {
                UsesCount += 1;
            }

            public void DecreaseUsagesCount()
            {
                if (UsesCount > 0) UsesCount -= 1;
            }

            public void GoToStartPosition() => bottleAnimationController.GoToStartPosition();
            public void GoUp()
            {
                bottleAnimationController.GoUp();
            }
        }
    }

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Core.InputSystem;
    using DG.Tweening;
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

            private int activeCoroutines;
            private bool sequenceCompleted;
            
            public void PouringColorsBetweenBottles(Bottle targetBottle, Action onComplete = null)
            {
                var countOfColorToTransfer = targetBottle.NumberOfColorToTransfer(GetNumberOfTopColorLayers());
                var colorToTransfer = GetTopColor();
                targetBottle.AddColor(countOfColorToTransfer, colorToTransfer);
                
                targetBottle.IncreaseUsagesCount();
                bottleAnimationController.ChooseRotationPointAndDirection(targetBottle.transform.position.x);
                InUse = true;
                sequenceCompleted = false;
                activeCoroutines = 0;
                SetSortingOrderUp();
                
                bottleAnimationController.PouringAnimation(targetBottle, colorToTransfer, countOfColorToTransfer,
                    shaderController, () =>
                    {
                        sequenceCompleted = true; 
                        CheckCompletion();
                        onComplete?.Invoke();
                    });
                
                /*
                var timeMove = bottleAnimationController.GetMoveTime();
                var timeRotation = bottleAnimationController.GetRotationTime();
                var pouringColors = DOTween.Sequence().SetTarget(gameObject);
                
                /*
                pouringColors.Append(transform.DOMove(targetBottle.transform.position + chosenPouringPoint, timeMove))
                    .InsertCallback(0.0f,
                        () => StartCoroutine(bottleAnimationController.RotateBottleBeforePouring(shaderController)))
                    .InsertCallback(timeMove + 0.01f,
                        () => StartCoroutine(bottleAnimationController.RotateBottleWithPouring(targetBottle, 
                            GetTopColor(), countOfColorToTransfer, shaderController)))
                    .Insert(timeMove + timeRotation + 0.02f, transform.DOMove(defaultPosition, timeMove))
                    .InsertCallback(timeMove + timeRotation + 0.02f,
                        () => StartCoroutine(TrackCoroutine(bottleAnimationController.RotateBottleBack(shaderController))))
                    .OnComplete(() => 
                    { 
                        sequenceCompleted = true; 
                        CheckCompletion();
                        onComplete?.Invoke();
                    });
                    */
            }

            private IEnumerator TrackCoroutine(IEnumerator coroutine)
            {
                activeCoroutines++;
                yield return StartCoroutine(coroutine);
                activeCoroutines--;
                CheckCompletion();
            }

            private void CheckCompletion()
            {
                if (activeCoroutines != 0 || !sequenceCompleted) return;
                InUse = false;
                SetSortingOrderDown();
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
            public void GoUp() => bottleAnimationController.GoUp();
        }
    }

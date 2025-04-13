using System;
using System.Collections;
using System.Collections.Generic;
using Core.InputSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace Scenes.GameScene.Bottle
{
    public class Bottle : MonoBehaviour, IClickable
    {
        [SerializeField] private BottleShaderController shaderController;
        [SerializeField] private PouringAnimationController pouringAnimationController;
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] private Transform rightPouringPoint;
        [SerializeField] private Transform leftPouringPoint;
        [SerializeField] private Transform rightRotationPoint;
        [SerializeField] private Transform leftRotationPoint;
        
        private Vector3 chosenPouringPoint;
        private Vector3 defaultPosition;
        public bool InUse { get; private set; }
        public int UsesCount { get; private set; }
        public event Action<Bottle> OnClicked;

        public void Initialize(List<Color> bottleColors)
        {
            shaderController.Initialize(bottleColors);
            SetDefaultPosition();
        }

        private void SetDefaultPosition()
        {
            defaultPosition = transform.position;
            Debug.Log($"def position in init {defaultPosition}");
        }
        
        public void OnClick()
        {
            OnClicked?.Invoke(this);
        }
    
        public void ChooseRotationPointAndDirection(float positionOfTargetBottleX)
        {
            if (transform.position.x > positionOfTargetBottleX && transform.localPosition.x <= 0.31) 
            {
                pouringAnimationController.SetRotationPoint(leftRotationPoint);
                chosenPouringPoint = rightPouringPoint.localPosition;
                pouringAnimationController.SetDirectionMultiplier(-1.0f);
            }
            else if (transform.localPosition.x >= -0.31)
            {
                pouringAnimationController.SetRotationPoint(rightRotationPoint);
                chosenPouringPoint = leftPouringPoint.localPosition;
                pouringAnimationController.SetDirectionMultiplier(1.0f);
            }
            else
            {
                pouringAnimationController.SetRotationPoint(leftRotationPoint);
                chosenPouringPoint = rightPouringPoint.localPosition;
                pouringAnimationController.SetDirectionMultiplier(-1.0f);
            }
        }
        
        public void GoUp()
        {
            transform.DOMoveY(defaultPosition.y + 0.20f, pouringAnimationController.GetMoveTime()).SetTarget(gameObject);
        }

        public void GoToStartPosition()
        {
            transform.DOMove(defaultPosition, pouringAnimationController.GetMoveTime()).SetTarget(gameObject);
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
            targetBottle.AddColor(countOfColorToTransfer, GetTopColor());
            
            targetBottle.IncreaseUsagesCount();
            ChooseRotationPointAndDirection(targetBottle.transform.position.x);
            InUse = true;
            sequenceCompleted = false;
            activeCoroutines = 0;
            SetSortingOrderUp();
            
            var timeMove = pouringAnimationController.GetMoveTime();
            var timeRotation = pouringAnimationController.GetRotationTime();
            var pouringColors = DOTween.Sequence().SetTarget(gameObject);
            
            pouringColors.Append(transform.DOMove(targetBottle.transform.position + chosenPouringPoint, timeMove))
                .InsertCallback(0.0f,
                    () => StartCoroutine(pouringAnimationController.RotateBottleBeforePouring(shaderController)))
                .InsertCallback(timeMove + 0.01f,
                    () => StartCoroutine(pouringAnimationController.RotateBottleWithPouring(targetBottle, 
                        GetTopColor(), countOfColorToTransfer, shaderController)))
                .Insert(timeMove + timeRotation + 0.02f, transform.DOMove(defaultPosition, timeMove))
                .InsertCallback(timeMove + timeRotation + 0.02f,
                    () => StartCoroutine(TrackCoroutine(pouringAnimationController.RotateBottleBack(shaderController))))
                .OnComplete(() => 
                { 
                    sequenceCompleted = true; 
                    CheckCompletion();
                    onComplete?.Invoke();
                });
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
            sortingGroup.sortingOrder = 1;
        }

        private void SetSortingOrderUp()
        {
            sortingGroup.sortingOrder = 2;
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

        public int GetNumberOfTopColorLayers()
        {
            return shaderController.NumberOfTopColorLayers;
        }

        public int NumberOfColorToTransfer(int countOfColor)
        {
            return shaderController.CalculateNumberOfColorsToTransfer(countOfColor);
        }

        public Stack<Color> GetColors()
        {
            return shaderController._bottleColors;
        }

        public void IncreaseUsagesCount()
        {
            UsesCount += 1;
        }

        public void DecreaseUsagesCount()
        {
            if (UsesCount > 0) UsesCount -= 1;
        }
    }
}

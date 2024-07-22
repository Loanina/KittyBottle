using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Scenes.GameScene
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField] private BottleShaderController shaderController;
        [SerializeField] private float[] rotationValues = {0.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;
        [SerializeField] private Transform rightPouringPoint;
        [SerializeField] private Transform leftPouringPoint;
        [SerializeField] private Transform rightRotationPoint;
        [SerializeField] private Transform leftRotationPoint;
        [SerializeField] private float timeRotation = 1.0f;
        [SerializeField] private float timeMove = 1.5f;
    
        private int rotationIndex;
        private float directionMultiplier = 1.0f;
        private Transform chosenRotationPoint;
        private Vector3 chosenPouringPoint;
        private Vector3 defaultPosition;
        
        public event Action<Bottle> OnClickEvent; 

        public void Initialize(List<Color> bottleColors)
        {
            shaderController.Initialize(bottleColors);
        }

        public void SetDefaultPosition()
        {
            defaultPosition = transform.position;
            Debug.Log($"def pos in init {defaultPosition}");
        }

        public void StartRotate(Bottle targetBottle, int countOfColorToTransfer)
        {
            StartCoroutine(RotateBottle(targetBottle, countOfColorToTransfer));
        }
    
        private IEnumerator RotateBottle(Bottle targetBottle, int countOfColorToTransfer)
        {
            rotationIndex = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            float time = 0;
            float angleValue;
            float lastAngleValue = 0;
            
            while (time < timeRotation)
            {
                var lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(0.0f,directionMultiplier * rotationValues[rotationIndex], lerpValue);
                transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue, lastAngleValue, targetBottle);

                time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
            angleValue = directionMultiplier * rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);
        }
    
        private IEnumerator RotateBottleBack()
        {
            float time = 0;
            float angleValue;
            var lastAngleValue = directionMultiplier * rotationValues[rotationIndex];
            
            while (time < timeRotation)
            {
                var lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(directionMultiplier * rotationValues[rotationIndex], 0.0f, lerpValue);
                transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShaderBack(angleValue);
                
                lastAngleValue = angleValue;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            angleValue = 0.0f;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            shaderController.RotateShaderBack(angleValue);
        }
    
        public void ChooseRotationPointAndDirection(float positionOfTargetBottleX)
        {
            if (transform.position.x > positionOfTargetBottleX)
            {
                chosenRotationPoint = leftRotationPoint;
                chosenPouringPoint = rightPouringPoint.localPosition;
                directionMultiplier = -1.0f;
            }
            else
            {
                chosenRotationPoint = rightRotationPoint;
                chosenPouringPoint = leftPouringPoint.localPosition;
                directionMultiplier = 1.0f;
            }
        }
        
        public void GoUp()
        {
            transform.DOMoveY(defaultPosition.y + 0.20f, timeMove);
        }

        public void GoToStartPosition()
        {
            transform.DOMove(defaultPosition, timeMove);
            Debug.Log($"go to default position --- {defaultPosition}");
        }
        
        public void FillUp(float fillUpToAdd)
        {
            shaderController.FillUp(fillUpToAdd);
        }

        public void AddColor(int count, Color color)
        {
            shaderController.AddColor(count, color);
        }

        public void PouringColorsBetweenBottles(Bottle targetBottle, int countOfColorToTransfer)
        {
            var pouringColors = DOTween.Sequence();
            pouringColors.Append(transform.DOMove(targetBottle.transform.position + chosenPouringPoint, timeMove))
                .InsertCallback(1, () => StartCoroutine(RotateBottle(targetBottle, countOfColorToTransfer)))
                .Insert(2, transform.DOMove(defaultPosition, timeMove))
                .InsertCallback(2, () => StartCoroutine(RotateBottleBack()));
        }

        public bool EnableToFillBottle(Color color)
        {
            return shaderController.EnableToFillBottle(color);
        }

        public Color GetTopColor()
        {
            return shaderController.TopColor;
        }

        public bool IsBottleEmpty()
        {
            return shaderController.IsBottleEmpty();
        }

        public int GetNumberOfTopColorLayers()
        {
            return shaderController.NumberOfTopColorLayers;
        }

        public int NumberOfColorToTransfer(int countOfColor)
        {
            return shaderController.CalculateNumberOfColorsToTransfer(countOfColor);
        }
        
        public void OnClick()
        {
            OnClickEvent?.Invoke(this);
        }
    }
}

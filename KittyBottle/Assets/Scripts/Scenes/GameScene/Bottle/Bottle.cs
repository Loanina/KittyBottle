using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scenes.GameScene.RaycastSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Scenes.GameScene.Bottle
{
    public class Bottle : MonoBehaviour, IClickable
    {
        [SerializeField] private BottleShaderController shaderController;
        [SerializeField] private PouringAnimationController pouringAnimationController;
        [SerializeField] private Transform glassTransform;
        [SerializeField] private float[] rotationValues = {33.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;
        [SerializeField] private Transform rightPouringPoint;
        [SerializeField] private Transform leftPouringPoint;
        [SerializeField] private Transform rightRotationPoint;
        [SerializeField] private Transform leftRotationPoint;
        [SerializeField] private float timeRotation = 0.7f;
        [SerializeField] private float timeMove = 0.5f;
    
        private int rotationIndex;
        private float directionMultiplier = 1.0f;
        private Transform chosenRotationPoint;
        private Vector3 chosenPouringPoint;
        private Vector3 defaultPosition;
        [HideInInspector]
        public bool InUse = false;
        [HideInInspector]
        public int UsesCount = 0;
        
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
    
        private IEnumerator RotateBottleWithPouring(Bottle targetBottle, int countOfColorToTransfer)
        {
            rotationIndex = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            var time = 0.0f;
            float angleValue;
            var firstAngleValue = glassTransform.transform.eulerAngles.z;
            if (firstAngleValue > 180)
            {
                firstAngleValue -= 360;
                firstAngleValue = Mathf.Abs(firstAngleValue);
            }
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;
            pouringAnimationController.DoColorFlow(directionMultiplier > 0.0f, GetTopColor());
            
            while (time < timeRotation)
            {
                var lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(firstAngleValue,directionMultiplier * rotationValues[rotationIndex], lerpValue);
                glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue, lastAngleValue, targetBottle);
                
                //time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                time += Time.deltaTime;
                
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
            pouringAnimationController.RemoveFlow(directionMultiplier > 0.0f);
            angleValue = directionMultiplier * rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);
            targetBottle.UsesCount -= 1;
        }

        private IEnumerator RotateBottleToAngle(float finishAngle)
        {
            float time = 0;
            float angleValue;
            float lastAngleValue = 0;
            while (time < timeMove)
            {
                var lerpValue = time / timeMove;
                angleValue = Mathf.Lerp(0.0f,finishAngle, lerpValue);
                glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue);
                
                time += Time.deltaTime;
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
        }
    
        private IEnumerator RotateBottleBack()
        {
            float time = 0;
            float angleValue;
            var firstAngleValue = glassTransform.transform.eulerAngles.z;
            if (firstAngleValue > 180) firstAngleValue -= 360;
            firstAngleValue = Mathf.Abs(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;
            
            while (time < timeMove)
            {
                var lerpValue = time / timeMove;
                angleValue = Mathf.Lerp(firstAngleValue, 0.0f, lerpValue);
                glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShaderBack(angleValue);
                
                lastAngleValue = angleValue;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            angleValue = 0.0f;
            glassTransform.eulerAngles = new Vector3(0, 0, angleValue);
            shaderController.RotateShaderBack(angleValue);
            InUse = false;
        }
    
        public void ChooseRotationPointAndDirection(float positionOfTargetBottleX)
        {
            if (transform.position.x > positionOfTargetBottleX && transform.localPosition.x <= 0.31) 
            {
                chosenRotationPoint = leftRotationPoint;
                chosenPouringPoint = rightPouringPoint.localPosition;
                directionMultiplier = -1.0f;
            }
            else if (transform.localPosition.x >= -0.31)
            {
                chosenRotationPoint = rightRotationPoint;
                chosenPouringPoint = leftPouringPoint.localPosition;
                directionMultiplier = 1.0f;
            }
            else
            {
                chosenRotationPoint = leftRotationPoint;
                chosenPouringPoint = rightPouringPoint.localPosition;
                directionMultiplier = -1.0f;
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
            var startPouringAngle = rotationValues[shaderController.CalculateStartPouringIndex()] * directionMultiplier;
            var pouringColors = DOTween.Sequence();
            
            pouringColors.Append(transform.DOMove(targetBottle.transform.position + chosenPouringPoint, timeMove))
                .InsertCallback(0.0f, () => StartCoroutine(RotateBottleToAngle(startPouringAngle)))
                .InsertCallback(timeMove + 0.01f, () => StartCoroutine(RotateBottleWithPouring(targetBottle, countOfColorToTransfer)))
                .Insert(timeMove + timeRotation + 0.02f, transform.DOMove(defaultPosition, timeMove))
                .InsertCallback(timeMove + timeRotation + 0.02f, () => StartCoroutine(RotateBottleBack()));
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

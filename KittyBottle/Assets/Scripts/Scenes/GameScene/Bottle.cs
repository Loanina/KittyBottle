using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene
{
    public class Bottle : MonoBehaviour
    {
        [SerializeField] private BottleShaderController shaderController;
        [SerializeField] private float timeRotation = 1.0f;
        [SerializeField] private float[] rotationValues = {0.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;
    
        private int rotationIndex;
    
        public Vector3 leftRotationPoint = new(-0.25f, 1.0f, 0.0f);
        public Vector3 rightRotationPoint = new(0.25f, 1.0f, 0.0f);
        private Vector3 chosenRotationPoint;
        private float directionMultiplier = 1.0f;

        public event Action<Bottle> OnClickEvent; 

        public void Initialize(List<Color> bottleColors)
        {
            shaderController.Initialize(bottleColors);
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
                transform.RotateAround(chosenRotationPoint, Vector3.forward, lastAngleValue - angleValue);
               
                shaderController.RotateShader(angleValue, lastAngleValue, targetBottle);

                time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
            angleValue = directionMultiplier * rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);

            StartCoroutine(RotateBottleBack());
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
                transform.RotateAround(chosenRotationPoint, Vector3.forward, lastAngleValue - angleValue);
                
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
                chosenRotationPoint = leftRotationPoint + transform.position;
                directionMultiplier = -1.0f;
            }
            else
            {
                chosenRotationPoint = rightRotationPoint + transform.position;
                directionMultiplier = 1.0f;
            }
        }
    
        public void FillUp(float fillUpToAdd)
        {
            shaderController.FillUp(fillUpToAdd);
        }

        public void AddColor(int count, Color color)
        {
            shaderController.AddColor(count, color);
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

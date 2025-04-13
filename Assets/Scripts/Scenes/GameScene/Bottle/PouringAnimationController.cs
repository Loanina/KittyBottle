using System.Collections;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class PouringAnimationController
    {
        private readonly PouringAnimationConfig config;
        private readonly BottleView view;
        private Transform chosenRotationPoint;
        private float directionMultiplier = 1.0f;

        [Inject]
        public PouringAnimationController(PouringAnimationConfig config, BottleView view)
        {
            this.config = config;
            this.view = view;
        }

        public void SetRotationPoint(Transform point)
        {
            chosenRotationPoint = point;
        }

        public void SetDirectionMultiplier(float value)
        {
            directionMultiplier = value;
        }

        public float GetRotationTime()
        {
            return config.timeRotation;
        }

        public float GetMoveTime()
        {
            return config.timeMove;
        }
        
        public IEnumerator RotateBottleBeforePouring(BottleShaderController shaderController)
        {
            var finishAngle = config.rotationValues[shaderController.CalculateStartPouringIndex()] * directionMultiplier;
            var time = 0.0f;
            float angleValue;
            var lastAngleValue = 0.0f;
            while (time < config.timeMove)
            {
                var lerpValue = time / config.timeMove;
                angleValue = Mathf.Lerp(0.0f,finishAngle, lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue);
                
                time += Time.deltaTime;
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
        }
        
        public IEnumerator RotateBottleWithPouring(Bottle targetBottle, Color colorToTransfer, int countOfColorToTransfer, BottleShaderController shaderController)
        {
            var rotationIndex = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            var time = 0.0f;
            float angleValue;
            var firstAngleValue = view.glassTransform.transform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;
            DoColorFlow(directionMultiplier > 0.0f, colorToTransfer);
            
            while (time < config.timeRotation)
            {
                var lerpValue = time / config.timeRotation;
                angleValue = Mathf.Lerp(firstAngleValue,directionMultiplier * config.rotationValues[rotationIndex], lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue, lastAngleValue, targetBottle);
                
                //time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                time += Time.deltaTime;
                
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }
            RemoveFlow(directionMultiplier > 0.0f);
            angleValue = directionMultiplier * config.rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);
            targetBottle.DecreaseUsagesCount();
        }
        
        public IEnumerator RotateBottleBack(BottleShaderController shaderController)
        {
            var time = 0.0f;
            float angleValue;
            var firstAngleValue = view.glassTransform.transform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;
            
            while (time < config.timeMove)
            {
                var lerpValue = time / config.timeMove;
                angleValue = Mathf.Lerp(firstAngleValue, 0.0f, lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShaderBack(angleValue);
                
                lastAngleValue = angleValue;
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            angleValue = 0.0f;
            view.glassTransform.eulerAngles = new Vector3(0, 0, angleValue);
            shaderController.RotateShaderBack(angleValue);
        }

        private static float CorrelatedEulerAngle(float angle)
        {
            while (angle > 180)
            {
                angle -= 360;
            }
            angle = Mathf.Abs(angle);
            return angle;
        }

        private void RemoveFlow(bool isRightPouringDirection)
        {
            if (isRightPouringDirection) view.rightColorFlow.enabled = false;
            else view.leftColorFlow.enabled = false;
        }

        private void DoColorFlow(bool isRightPouringDirection, Color color)
        {
            if (isRightPouringDirection)
            {
                view.rightColorFlow.enabled = true;
                view.rightColorFlow.color = color;
            }
            else
            {
                view.leftColorFlow.enabled = true;
                view.leftColorFlow.color = color;
            }
        }
    }
}

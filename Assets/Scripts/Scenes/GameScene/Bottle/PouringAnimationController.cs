using System.Collections;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class PouringAnimationController : MonoBehaviour
    {
        [SerializeField] private float[] rotationValues = {33.0f, 67.5f, 78.75f, 85.6f, 90.0f};
        [SerializeField] private float timeRotation = 0.7f;
        [SerializeField] private float timeMove = 0.5f;
        [SerializeField] private Transform glassTransform;
        [SerializeField] private SpriteRenderer rightColorFlow;
        [SerializeField] private SpriteRenderer leftColorFlow;
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;
        private Transform chosenRotationPoint;
        private float directionMultiplier = 1.0f;

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
            return timeRotation;
        }

        public float GetMoveTime()
        {
            return timeMove;
        }
        
        public IEnumerator RotateBottleBeforePouring(BottleShaderController shaderController)
        {
            var finishAngle = rotationValues[shaderController.CalculateStartPouringIndex()] * directionMultiplier;
            var time = 0.0f;
            float angleValue;
            var lastAngleValue = 0.0f;
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
        
        public IEnumerator RotateBottleWithPouring(Bottle targetBottle, Color colorToTransfer, int countOfColorToTransfer, BottleShaderController shaderController)
        {
            var rotationIndex = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            var time = 0.0f;
            float angleValue;
            var firstAngleValue = glassTransform.transform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;
            DoColorFlow(directionMultiplier > 0.0f, colorToTransfer);
            
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
            RemoveFlow(directionMultiplier > 0.0f);
            angleValue = directionMultiplier * rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);
            targetBottle.DecreaseUsagesCount();
        }
        
        public IEnumerator RotateBottleBack(BottleShaderController shaderController)
        {
            var time = 0.0f;
            float angleValue;
            var firstAngleValue = glassTransform.transform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
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
            if (isRightPouringDirection) rightColorFlow.enabled = false;
            else leftColorFlow.enabled = false;
        }

        private void DoColorFlow(bool isRightPouringDirection, Color color)
        {
            if (isRightPouringDirection)
            {
                rightColorFlow.enabled = true;
                rightColorFlow.color = color;
            }
            else
            {
                leftColorFlow.enabled = true;
                leftColorFlow.color = color;
            }
        }
    }
}

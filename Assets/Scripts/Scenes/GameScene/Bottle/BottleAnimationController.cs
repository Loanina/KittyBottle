using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottleAnimationController
    {
        private readonly BottleAnimationConfig config;
        private readonly BottleShaderController shaderController;
        private readonly BottleView view;
        private readonly Transform bottleTransform;
        private Vector3 chosenPouringPoint;
        private Transform chosenRotationPoint;
        private Vector3 defaultPosition;
        private float directionMultiplier = 1.0f;

        [Inject]
        public BottleAnimationController(BottleShaderController shaderController, BottleAnimationConfig config, BottleView view, Transform bottleTransform)
        {
            this.config = config;
            this.view = view;
            this.bottleTransform = bottleTransform;
            this.shaderController = shaderController;
        }
        
        public void SetDefaultPosition()
        {
            defaultPosition = bottleTransform.position;
            Debug.Log($"def position in init {defaultPosition}");
        }

        private async UniTask RotateBottleBeforePouringAsync(CancellationToken cancellationToken)
        {
            var finishAngle = config.rotationValues[shaderController.CalculateStartPouringIndex()] * directionMultiplier;
            var time = 0.0f;
            float angleValue;
            var lastAngleValue = 0.0f;

            while (time < config.timeMove)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lerpValue = time / config.timeMove;
                angleValue = Mathf.Lerp(0.0f, finishAngle, lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue);

                time += Time.deltaTime;
                lastAngleValue = angleValue;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            }
        }

        private async UniTask RotateBottleWithPouringAsync(Bottle targetBottle, Color colorToTransfer, int countOfColorToTransfer,
            CancellationToken cancellationToken)
        {
            var rotationIndex = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            var time = 0.0f;
            float angleValue;

            var firstAngleValue = view.glassTransform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;

            view.SetColorFlow(directionMultiplier > 0.0f, true, colorToTransfer);

            while (time < config.timeRotation)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lerpValue = time / config.timeRotation;
                angleValue = Mathf.Lerp(firstAngleValue, directionMultiplier * config.rotationValues[rotationIndex], lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShader(angleValue, lastAngleValue, targetBottle);
                //time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                time += Time.deltaTime;
                lastAngleValue = angleValue;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            }

            view.SetColorFlow(directionMultiplier > 0.0f, false);

            angleValue = directionMultiplier * config.rotationValues[rotationIndex];
            shaderController.RotateShaderComplete(angleValue, countOfColorToTransfer);
        }

        private async UniTask RotateBottleBackAsync(CancellationToken cancellationToken)
        {
            var time = 0.0f;
            float angleValue;

            var firstAngleValue = view.glassTransform.eulerAngles.z;
            firstAngleValue = CorrelatedEulerAngle(firstAngleValue);
            firstAngleValue *= directionMultiplier;
            var lastAngleValue = firstAngleValue;

            while (time < config.timeMove)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var lerpValue = time / config.timeMove;
                angleValue = Mathf.Lerp(firstAngleValue, 0.0f, lerpValue);
                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                shaderController.RotateShaderBack(angleValue);

                time += Time.deltaTime;
                lastAngleValue = angleValue;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
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

        private void ChooseRotationPointAndDirection(float positionOfTargetBottleX)
        {
            if (bottleTransform.position.x > positionOfTargetBottleX && bottleTransform.localPosition.x <= config.leftEdgeThreshold) 
            {
                chosenRotationPoint = view.leftRotationPoint;
                chosenPouringPoint = view.rightPouringPoint.localPosition;
                directionMultiplier = -1.0f;    
            }
            else if (bottleTransform.localPosition.x >= -1 * config.leftEdgeThreshold)
            {
                chosenRotationPoint = view.rightRotationPoint;
                chosenPouringPoint = view.leftPouringPoint.localPosition;
                directionMultiplier = 1.0f;
            }
            else
            {
                chosenRotationPoint = view.leftRotationPoint;
                chosenPouringPoint = view.rightPouringPoint.localPosition;
                directionMultiplier = -1.0f;
            }
        }
        
        public void GoUp()
        {
            bottleTransform.DOMoveY(defaultPosition.y + config.upOffset, config.timeMove).SetTarget(bottleTransform);
        }

        public void GoToStartPosition()
        {
            bottleTransform.DOMove(defaultPosition, config.timeMove).SetTarget(bottleTransform);
        }

        public async UniTask PouringAnimationAsync(
            Bottle targetBottle,
            Color colorToTransfer,
            int countOfColorToTransfer,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ChooseRotationPointAndDirection(targetBottle.transform.position.x);
                
                // 1. Двигаем бутылку и одновременно вращаем перед началом
                var moveToTarget = bottleTransform
                    .DOMove(targetBottle.transform.position + chosenPouringPoint, config.timeMove)
                    .ToUniTask(cancellationToken: cancellationToken);

                var rotateBefore = RotateBottleBeforePouringAsync(cancellationToken);

                await UniTask.WhenAll(moveToTarget, rotateBefore);

                // 2. Вращение с наливанием
                await RotateBottleWithPouringAsync(targetBottle, colorToTransfer, countOfColorToTransfer, cancellationToken);
                targetBottle.DecreaseUsagesCount();

                // 3. Возвращаем бутылку и крутим обратно одновременно
                var moveBack = bottleTransform
                    .DOMove(defaultPosition, config.timeMove)
                    .ToUniTask(cancellationToken: cancellationToken);

                var rotateBack = RotateBottleBackAsync(cancellationToken);

                await UniTask.WhenAll(moveBack, rotateBack);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("$[PouringAnimation] cancelled correctly");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PouringAnimation] Unexpected error: {ex}");
            }
        }

    }
}

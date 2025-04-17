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
        
        private async UniTask RotateBottleAsync(
            float fromAngle,
            float toAngle,
            float duration,
            CancellationToken cancellationToken,
            Action<float, float>? perFrameAction = null,
            Action? beforeRotation = null,
            Action? afterRotation = null,
            PlayerLoopTiming loopTiming = PlayerLoopTiming.LastPostLateUpdate)
        {
            beforeRotation?.Invoke();

            var time = 0f;
            float angleValue;
            var lastAngleValue = fromAngle;

            while (time < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var t = time / duration;
                angleValue = Mathf.Lerp(fromAngle, toAngle, t);

                view.glassTransform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
                perFrameAction?.Invoke(angleValue, lastAngleValue);

                lastAngleValue = angleValue;
                time += Time.deltaTime;

                await UniTask.Yield(loopTiming, cancellationToken);
            }

            afterRotation?.Invoke();
        }


        private async UniTask RotateBottleBeforePouringAsync(CancellationToken cancellationToken)
        {
            var finishAngle = config.rotationValues[shaderController.CalculateStartPouringIndex()] * directionMultiplier;

            await RotateBottleAsync(
                fromAngle: 0f,
                toAngle: finishAngle,
                duration: config.timeMove,
                cancellationToken: cancellationToken,
                perFrameAction: (angle, _) => shaderController.RotateShader(angle)
            );
        }
        
        private async UniTask RotateBottleWithPouringAsync(
            Bottle targetBottle,
            Color colorToTransfer,
            int countOfColorToTransfer,
            CancellationToken cancellationToken)
        {
            var index = shaderController.CalculateRotationIndexToAnotherBottle(countOfColorToTransfer);
            var targetAngle = directionMultiplier * config.rotationValues[index];
            var startAngle = CorrelatedEulerAngle(view.glassTransform.eulerAngles.z) * directionMultiplier;

            await RotateBottleAsync(
                fromAngle: startAngle,
                toAngle: targetAngle,
                duration: config.timeRotation,
                cancellationToken: cancellationToken,
                beforeRotation: () => view.SetColorFlow(directionMultiplier > 0f, true, colorToTransfer),
                perFrameAction: (angle, lastAngle) => shaderController.RotateShader(angle, lastAngle, targetBottle),
                afterRotation: () =>
                {
                    view.SetColorFlow(directionMultiplier > 0f, false);
                    shaderController.RotateShaderComplete(targetAngle, countOfColorToTransfer);
                }
            );
        }

        private async UniTask RotateBottleBackAsync(CancellationToken cancellationToken)
        {
            var startAngle = CorrelatedEulerAngle(view.glassTransform.eulerAngles.z) * directionMultiplier;

            await RotateBottleAsync(
                fromAngle: startAngle,
                toAngle: 0f,
                duration: config.timeMove,
                cancellationToken: cancellationToken,
                perFrameAction: (angle, _) => shaderController.RotateShaderBack(angle),
                afterRotation: () =>
                {
                    view.glassTransform.eulerAngles = Vector3.zero;
                    shaderController.RotateShaderBack(0f);
                }
            );
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

        private enum RotationDirection
        {
            Left = -1,
            Right = 1
        }

        private void SetDirection(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                chosenRotationPoint = view.rightRotationPoint;
                chosenPouringPoint = view.leftPouringPoint.localPosition;
            }
            else
            {
                chosenRotationPoint = view.leftRotationPoint;
                chosenPouringPoint = view.rightPouringPoint.localPosition;
            }
            directionMultiplier = (float)direction;
        }

        private void ChooseRotationPointAndDirection(float positionOfTargetBottleX)
        {
            if (positionOfTargetBottleX >= config.edgeThreshold)
                SetDirection(RotationDirection.Right);
            else if (positionOfTargetBottleX <= -config.edgeThreshold)
                SetDirection(RotationDirection.Left);
            else if (bottleTransform.position.x >= positionOfTargetBottleX)
                SetDirection(RotationDirection.Left);
            else
                SetDirection(RotationDirection.Right);
        }
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scenes.GameScene.Bottle.Shader;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle.Animation
{
    public partial class BottleAnimationController
    {
        private readonly BottleAnimationConfig config;
        private readonly BottleShaderController shaderController;
        private readonly BottleView view;
        private readonly Transform bottleTransform;
        private Vector3 defaultPosition;

        [Inject]
        public BottleAnimationController(BottleShaderController shaderController, BottleAnimationConfig config, BottleView view, Transform bottleTransform)
        {
            this.config = config;
            this.view = view;
            this.bottleTransform = bottleTransform;
            this.shaderController = shaderController;
        }

        public void SetDefaultPosition() => defaultPosition = bottleTransform.position;

        private RotationData GetRotationData(float targetBottleX)
        {
            bool isClockwise;

            if (targetBottleX >= config.edgeThreshold)
                isClockwise = true;
            else if (targetBottleX <= -config.edgeThreshold)
                isClockwise = false;
            else
                isClockwise = bottleTransform.position.x < targetBottleX;

            return isClockwise
                ? new RotationData(view.rightRotationPoint, view.leftPouringPoint.localPosition, true)
                : new RotationData(view.leftRotationPoint, view.rightPouringPoint.localPosition, false);
        }

        private async UniTask RotateAsync(
            float fromAngle,
            float toAngle,
            float duration,
            Transform rotationPoint,
            Func<float, float, Bottle, UniTask> onRotate,
            Bottle targetBottle,
            CancellationToken cancellationToken)
        {
            var time = 0f;
            var lastAngle = fromAngle;

            while (time < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var t = time / duration;
                var angle = Mathf.Lerp(fromAngle, toAngle, t);

                view.glassTransform.RotateAround(rotationPoint.position, Vector3.forward, lastAngle - angle);
                await onRotate.Invoke(angle, lastAngle, targetBottle);

                lastAngle = angle;
                time += Time.deltaTime;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
            }
        }

        private static float CorrelatedEulerAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            return Mathf.Abs(angle);
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
            var isRotateCompleteCalled = false;
            var isDecreaseUsageCalled = false;
            try
            {
                var rotationData = GetRotationData(targetBottle.transform.position.x);

                // Move to target
                var moveToTarget = bottleTransform
                    .DOMove(targetBottle.transform.position + rotationData.PouringOffset, config.timeMove)
                    .ToUniTask(cancellationToken: cancellationToken);

                var rotateBefore = RotateAsync(
                    0f,
                    config.rotationValues[shaderController.CalculateStartPouringIndex()] * (rotationData.IsClockwise ? 1f : -1f),
                    config.timeMove,
                    rotationData.RotationPoint,
                    async (angle, _, _) =>
                    {
                        shaderController.Rotate(angle);
                        await UniTask.CompletedTask;
                    },
                    null,
                    cancellationToken
                );

                await UniTask.WhenAll(moveToTarget, rotateBefore);

                // Pouring rotation
                var rotationIndex = shaderController.CalculateRotationIndex(countOfColorToTransfer);
                var fromAngle = CorrelatedEulerAngle(view.glassTransform.eulerAngles.z) * (rotationData.IsClockwise ? 1f : -1f);
                var toAngle = config.rotationValues[rotationIndex] * (rotationData.IsClockwise ? 1f : -1f);

                view.SetColorFlow(rotationData.IsClockwise, true, colorToTransfer);

                await RotateAsync(
                    fromAngle,
                    toAngle,
                    config.timeRotation,
                    rotationData.RotationPoint,
                    async (angle, lastAngle, target) =>
                    {
                        shaderController.Rotate(angle, lastAngle, target);
                        await UniTask.CompletedTask;
                    },
                    targetBottle,
                    cancellationToken
                );

                view.SetColorFlow(rotationData.IsClockwise, false);
                shaderController.RotateComplete(toAngle);
                shaderController.RemoveTopColor(countOfColorToTransfer);
                isRotateCompleteCalled = true;
                targetBottle.DecreaseUsagesCount();
                isDecreaseUsageCalled = true;

                // Move and rotate back
                var moveBack = bottleTransform
                    .DOMove(defaultPosition, config.timeMove)
                    .ToUniTask(cancellationToken: cancellationToken);

                await UniTask.WhenAll(
                    moveBack,
                    RotateAsync(
                        toAngle,
                        0f,
                        config.timeMove,
                        rotationData.RotationPoint,
                        async (angle, _, _) =>
                        {
                            shaderController.Rotate(angle);
                            await UniTask.CompletedTask;
                        },
                        null,
                        cancellationToken
                    )
                );

                view.glassTransform.eulerAngles = Vector3.zero;
                shaderController.Rotate(0f);
            }
            catch (OperationCanceledException)
            {
                RestoreDefaultState(isRotateCompleteCalled, isDecreaseUsageCalled, targetBottle, countOfColorToTransfer);
                Debug.Log("[PouringAnimation] cancelled correctly");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PouringAnimation] Unexpected error: {ex}");
            }
        }

        private void RestoreDefaultState(bool isRotateCompleteCalled, bool isDecreaseUsageCalled, Bottle targetBottle, int colorCountToTransfer)
        {
            if (!isRotateCompleteCalled)
            {
                shaderController.RotateComplete(0f);
                shaderController.RemoveTopColor(colorCountToTransfer);
            }
            if (!isDecreaseUsageCalled) targetBottle.DecreaseUsagesCount();
            
            DOTween.Kill(bottleTransform);
            bottleTransform.position = defaultPosition;
            view.glassTransform.eulerAngles = Vector3.zero;
            view.glassTransform.localPosition = Vector3.zero;
            view.StopColorFlow();
            shaderController.Rotate(0f);
            targetBottle.UpdateFillAmount();
        }
    }
}
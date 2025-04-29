using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class PouringService
    {
        public async UniTask TransferAsync(
            Bottle source,
            Bottle target,
            Color color,
            int count,
            Action onComplete = null)
        {
            target.AddColor(color, count, false);
            target.IncreaseUsageCount();

            source.StartUse();
            source.SetSortingOrder(true);
            source.SetPouring(true);
            
            try
            {
                await source.PouringAnimationAsync(target, color, count);
                onComplete?.Invoke();
            }
            finally
            {
                source.EndUse();
                source.SetSortingOrder(false);
                source.SetPouring(false);
            }
        }
    }
}
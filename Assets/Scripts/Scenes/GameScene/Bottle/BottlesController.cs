using System;
using System.Collections.Generic;
using Core.Hints;
using Core.Hints.Moves;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesController : IInitializable, IDisposable
    {
        private readonly BottlesContainer bottlesContainer;
        private readonly HintManager hintManager;
        private Bottle firstBottle;
        private Bottle secondBottle;

        [Inject]
        public BottlesController(BottlesContainer bottlesContainer, HintManager hintManager)
        {
            this.bottlesContainer = bottlesContainer;
            this.hintManager = hintManager;
        }

        public void Dispose()
        {
            bottlesContainer.OnBottlesCreated -= OnBottlesCreated;
            bottlesContainer.OnBottlesDeleted -= OnBottlesDeleted;
            hintManager.OnReturnMove -= ReturnMove;
        }

        public void Initialize()
        {
            bottlesContainer.OnBottlesCreated += OnBottlesCreated;
            bottlesContainer.OnBottlesDeleted += OnBottlesDeleted;
            hintManager.OnReturnMove += ReturnMove;
            if (bottlesContainer.GetAllBottles()?.Count > 0)
                OnBottlesCreated(bottlesContainer.GetAllBottles());
        }

        public event Action OnPouringEnd;

        private void OnBottlesCreated(List<Bottle> bottles)
        {
            foreach (var bottle in bottles) bottle.OnClicked += OnBottleClicked;
        }

        private void OnBottlesDeleted(List<Bottle> bottles)
        {
            foreach (var bottle in bottles) bottle.OnClicked -= OnBottleClicked;
            ClearSelection();
            hintManager.ClearMoveHistory();
        }

        private void OnBottleClicked(Bottle bottle)
        {
            if (bottle == null) return;
            if (firstBottle == null)
            {
                if (bottle.InUse || bottle.UsesCount > 0) return;
                firstBottle = bottle;
                firstBottle.GoUp();
                Debug.Log("First bottle selected");
            }
            else if (firstBottle == bottle)
            {
                firstBottle.GoToStartPosition();
                ClearSelection();
                Debug.Log("Deselected the same bottle");
            }
            else
            {
                if (bottle.InUse) return;
                secondBottle = bottle;
                Debug.Log("Second bottle selected");

                TransferColor(firstBottle, secondBottle);
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            firstBottle = null;
            secondBottle = null;
        }

        private bool CanTransferColor(Bottle from, Bottle to) => !from.IsEmpty() && to.CanFill(from.GetTopColor());

        private void TransferColor(Bottle from, Bottle to)
        {
            if (!CanTransferColor(from, to))
            {
                from.GoToStartPosition();
                return;
            }
            
            hintManager.AddMove(
                bottlesContainer.GetIndexOfBottle(from),
                bottlesContainer.GetIndexOfBottle(to),
                to.GetTransferableCount(from.GetTopColorLayers())
            );

            from.PourColorsAsync(to, OnPouringEnd).Forget();
        }

        private async void ReturnMove(Move move)
        {
            var bottleFrom = bottlesContainer.GetBottle(move.From);
            var bottleTo = bottlesContainer.GetBottle(move.To);
            
            try
            {
                await UniTask.WhenAll(
                    bottleFrom.CancelAnimationAsync(),
                    bottleTo.CancelAnimationAsync()
                );

                TransferColorWithoutAnimation(bottleTo, bottleFrom, move.TransferAmount);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[ReturnMove] The animation was canceled during the turn back. No further actions are performed.");
            }
        }

        private void TransferColorWithoutAnimation(Bottle from, Bottle to, int countOfColorToTransfer)
        {
            to.AddColor(from .GetTopColor(), countOfColorToTransfer, true);
            from.RemoveTopColor(countOfColorToTransfer);
        }
    }
}
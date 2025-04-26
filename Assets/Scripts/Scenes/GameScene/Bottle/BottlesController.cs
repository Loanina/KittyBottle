using System;
using System.Collections.Generic;
using Core.Hints;
using Core.Hints.Moves;
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

                TransferColor();
                ClearSelection();
            }
        }

        private void ClearSelection()
        {
            firstBottle = null;
            secondBottle = null;
        }

        private bool CanTransferColor() => !firstBottle.IsEmpty() && secondBottle.EnableToFill(firstBottle.GetTopColor());

        private void TransferColor()
        {
            if (!CanTransferColor())
            {
                firstBottle.GoToStartPosition();
                return;
            }
            
            hintManager.AddMove(
                bottlesContainer.GetIndexOfBottle(firstBottle),
                bottlesContainer.GetIndexOfBottle(secondBottle),
                secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers())
            );

            firstBottle.PouringColorsBetweenBottles(secondBottle, () =>
            {
                OnPouringEnd?.Invoke();
            });
        }

        private void ReturnMove(Move move) => TransferColorWithoutAnimation(move.To, move.From, move.TransferAmount);

        private void TransferColorWithoutAnimation(int from, int to, int countOfColorToTransfer)
        {
            var bottleFrom = bottlesContainer.GetBottle(from);
            var bottleTo = bottlesContainer.GetBottle(to);
            bottleTo.AddColor(bottleFrom.GetTopColor(), countOfColorToTransfer, true);
            bottleFrom.RemoveTopColor(countOfColorToTransfer);
        }
    }
}
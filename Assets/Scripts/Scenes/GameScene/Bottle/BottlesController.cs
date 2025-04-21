using System;
using Scenes.GameScene.Bottle.Moves;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesController : IInitializable, IDisposable
    {
        private readonly BottlesContainer bottlesContainer;
        private readonly MovesManager movesManager;
        private Bottle firstBottle;
        private Bottle secondBottle;
        
        public event Action OnPouringEnd;
        
        [Inject]
        public BottlesController(BottlesContainer bottlesContainer, MovesManager movesManager)
        {
            this.bottlesContainer = bottlesContainer;
            this.movesManager = movesManager;
        }
        
        public void Initialize()
        {
            bottlesContainer.OnBottlesCreated += OnBottlesCreated;
            bottlesContainer.OnBottlesDeleted += OnBottlesDeleted;
            if (bottlesContainer.GetAllBottles()?.Count > 0)
                OnBottlesCreated(bottlesContainer.GetAllBottles());
        }

        public void Dispose()
        {
            bottlesContainer.OnBottlesCreated -= OnBottlesCreated;
            bottlesContainer.OnBottlesDeleted -= OnBottlesDeleted;
        }
        
        private void OnBottlesCreated(System.Collections.Generic.List<Bottle> bottles)
        {
            foreach (var bottle in bottles)
            {
                bottle.OnClicked += OnBottleClicked;
            }
        }
        
        private void OnBottlesDeleted()
        {
            ClearSelection();
        }

        private void OnBottleClicked(Bottle bottle)
        {
            try
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

                    Debug.Log("Second bottle selected");
                    secondBottle = bottle;
                    TransferColor();
                    ClearSelection();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error handling bottle click: {e}");
                throw;
            }
        }

        private void ClearSelection()
        {
            firstBottle = null;
            secondBottle = null;
        }
        
        private bool CanTransferColor()
        {
            var isFirstBottleEmpty = firstBottle.IsEmpty();
            var canFillSecondBottle = secondBottle.EnableToFill(firstBottle.GetTopColor());
            
            Debug.Log($"First bottle empty: {isFirstBottleEmpty}, Can fill second: {canFillSecondBottle}");
            return !isFirstBottleEmpty && canFillSecondBottle;
        }
        
        private void TransferColor()
        {
            if (!CanTransferColor())
            {
                firstBottle.GoToStartPosition();
                return;
            }

            // var transferAmount = secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers());
            
            firstBottle.PouringColorsBetweenBottles(secondBottle, () => 
            {
                /*
                movesManager.AddMove(
                    bottlesContainer.GetIndexOfBottle(firstBottle),
                    bottlesContainer.GetIndexOfBottle(secondBottle),
                    transferAmount
                );
                */
                OnPouringEnd?.Invoke();
            });
        }

        private void ReturnMove()
        {
            var lastMove = movesManager.PopLastMove();
            TransferColorWithoutAnimation(lastMove.from, lastMove.to, lastMove.countOfColorToTransfer);
        }

        private void TransferColorWithoutAnimation(int from, int to, int countOfColorToTransfer)
        {
            var bottleFrom = bottlesContainer.GetBottle(from);
            var bottleTo = bottlesContainer.GetBottle(to);
            bottleFrom.AddColor(bottleTo.GetTopColor(), countOfColorToTransfer);
            bottleTo.RemoveTopColor(countOfColorToTransfer);
        }
    }
}

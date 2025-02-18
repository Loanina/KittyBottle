using System;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Bottle
{
    public class BottlesController
    {
        private readonly BottlesContainer bottlesContainer;
        private readonly MovesManager movesManager;
        private Bottle firstBottle;
        private Bottle secondBottle;
        
        [Inject]
        public BottlesController(BottlesContainer container, MovesManager movesManager)
        {
            bottlesContainer = container;
            this.movesManager = movesManager;
        }

        public void PeekBottle(Bottle bottle)
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
                    if (bottle.InUse)
                    {
                        Debug.Log("Target bottle is in use");
                        return;
                    }

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
            var canFillSecondBottle = secondBottle.EnableToFillBottle(firstBottle.GetTopColor());
            
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

            var transferAmount = secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers());
            secondBottle.IncreaseUsagesCount();
            
            firstBottle.PouringColorsBetweenBottles(secondBottle, () => 
            {
                movesManager.AddMove(
                    bottlesContainer.GetIndexOfBottle(firstBottle),
                    bottlesContainer.GetIndexOfBottle(secondBottle),
                    transferAmount
                );
                ClearSelection();
            });
        }
        
        public void TransferColorWithoutAnimation(int from, int to, int countOfColorToTransfer)
        {
            var bottleFrom = bottlesContainer.GetBottle(from);
            var bottleTo = bottlesContainer.GetBottle(to);
            bottleFrom.AddColor(countOfColorToTransfer, bottleTo.GetTopColor());
            bottleTo.RemoveTopColor(countOfColorToTransfer);
        }
    }
}

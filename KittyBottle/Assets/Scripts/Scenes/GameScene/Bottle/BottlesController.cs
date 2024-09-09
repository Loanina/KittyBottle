using System;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class BottlesController : MonoBehaviour
    {
        private BottlesContainer bottlesContainer;
        private MovesManager movesManager;
        private Bottle firstBottle;
        private Bottle secondBottle;

        public void OnClickBottle(Bottle bottle)
        {
            try
            {
                if (bottle == null) return;
                if (firstBottle == null)
                {
                    if (bottle.InUse || bottle.UsesCount > 0) return;
                    firstBottle = bottle;
                    firstBottle.GoUp();
                    Debug.Log("first bottle selected");
                }
                else if (firstBottle == bottle)
                {
                    firstBottle.GoToStartPosition();
                    ClearSelection();
                    Debug.Log("selected bottle equals");
                }
                else
                {
                    if (bottle.InUse)
                    {
                        Debug.Log("BOTTlE IN USE ohhhhh ssshhhhiiiiiitttt");
                        return;
                    }
                    Debug.Log("2 bottles selected");
                    secondBottle = bottle;
                    TransferColor();
                    ClearSelection();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }

        private void ClearSelection()
        {
            firstBottle = null;
            secondBottle = null;
        }

        private bool EnableToTransferColor()
        {
            Debug.Log(
                $"is empty {firstBottle.IsBottleEmpty()} enable to fill second bottle {secondBottle.EnableToFillBottle(firstBottle.GetTopColor())}");
            return !firstBottle.IsBottleEmpty() && secondBottle.EnableToFillBottle(firstBottle.GetTopColor());
        }

        private void TransferColor()
        {
            if (!EnableToTransferColor())
            {
                firstBottle.GoToStartPosition();
                return;
            }
            firstBottle.InUse = true;
            secondBottle.UsesCount += 1;
            var countOfColorToTransfer = secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers());
            secondBottle.AddColor(countOfColorToTransfer, firstBottle.GetTopColor());
            firstBottle.ChooseRotationPointAndDirection(secondBottle.transform.position.x);
            firstBottle.PouringColorsBetweenBottles(secondBottle, countOfColorToTransfer);
            
       //     movesManager.AddMove(bottlesContainer.GetIndexOfBottle(firstBottle), bottlesContainer.GetIndexOfBottle(secondBottle), countOfColorToTransfer);
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

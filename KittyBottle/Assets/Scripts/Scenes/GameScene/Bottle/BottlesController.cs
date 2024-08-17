using System;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class BottlesController : MonoBehaviour
    {
        private Bottle firstBottle;
        private Bottle secondBottle;

        public void OnClickBottle(Bottle bottle)
        {
            try
            {
                if (bottle == null)
                {
                    Debug.Log("bottle empty");
                    return;
                }
                if (firstBottle == null)
                {
                    if (bottle.InUse)
                    {
                        Debug.Log("selected bottle in use");
                        return;
                    }
                    if (bottle.IsFrozen)
                    {
                        Debug.Log("selected bottle is frozen");
                        return;
                    }
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
                        Debug.Log("selected bottle in use");
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
            Debug.Log("first bottle in use");
            secondBottle.IsFrozen = true;
            Debug.Log("second bottle frozen");
            
            var countOfColorToTransfer = secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers());
            Debug.Log($"count of colors to transfer {countOfColorToTransfer}");
            secondBottle.AddColor(countOfColorToTransfer, firstBottle.GetTopColor());
            firstBottle.ChooseRotationPointAndDirection(secondBottle.transform.position.x);
            firstBottle.PouringColorsBetweenBottles(secondBottle, countOfColorToTransfer);
        }
    }
}

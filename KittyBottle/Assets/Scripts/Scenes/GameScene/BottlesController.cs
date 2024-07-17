using System;
using UnityEngine;

namespace Scenes.GameScene
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

            var countOfColorToTransfer = secondBottle.NumberOfColorToTransfer(firstBottle.GetNumberOfTopColorLayers());
            Debug.Log($"count of colors to transfer {countOfColorToTransfer}");
            
            secondBottle.AddColor(countOfColorToTransfer, firstBottle.GetTopColor());
            firstBottle.ChooseRotationPointAndDirection(secondBottle.transform.position.x);
            firstBottle.PouringColorsBetweenBottles(secondBottle, countOfColorToTransfer);
        }
    }
}

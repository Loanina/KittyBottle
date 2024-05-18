using System;
using UnityEngine;

public class BottlesController : MonoBehaviour
{
    private Bottle firstBottle;
    private Bottle secondBottle;

    public void OnClickBottle(Bottle bottle)
    {
        try
        {
            Debug.Log("Нажалось");
            if (bottle == null) return;
            if (firstBottle == null)
            {
                firstBottle = bottle;
            }
            else if (firstBottle == bottle)
            {
                ClearSelection();
            }
            else
            {
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
        return !firstBottle.IsBottleEmpty() && secondBottle.EnableToFillBottle(firstBottle.GetTopColor());
    }

    private void TransferColor()
    {
        if (!EnableToTransferColor()) return;
        secondBottle.AddColor(firstBottle.GetNumberOfTopColorLayers(), firstBottle.GetTopColor());
            
        firstBottle.ChooseRotationPointAndDirection(secondBottle.transform.position.x);
        firstBottle.StartRotate(secondBottle);
    }
}

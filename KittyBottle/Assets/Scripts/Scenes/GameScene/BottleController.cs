using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Scenes.GameScene
{
    public class BottleController : MonoBehaviour
    {
        [SerializeField] private List<Color> bottleColors;
        [SerializeField] private SpriteRenderer bottleMaskSR;
        

        [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
        [SerializeField] private AnimationCurve fillAmountCurve;
        

        [SerializeField] private float[] fillAmountValues;
        
        
        //потом вынести с отдельными кривыми и размерами бутылок для каждого
        [SerializeField] private int maxColorsInBottle = 4;
        
       
        private Color topColor;
        private int numberOfTopColorLayers = 1;

        //можно будет убрать
        [SerializeField] private BottleController bottleControllerRef;
        [SerializeField] private bool justThisBottle = false;
        private int numberOfColorsToTransfer = 0;


        private void UpdateTopColorValues()
        {
            if (bottleColors.Count == 0)
            {
                numberOfTopColorLayers = 0;
            }
            else
            {
                numberOfTopColorLayers = 1;
                topColor = bottleColors[^1];
                
                if (bottleColors.Count > 1)
                {
                    for (var i = bottleColors.Count - 1; i > 0; i--)
                    {
                        if (bottleColors[i].Equals(bottleColors[i - 1]))
                        {
                            numberOfTopColorLayers++;
                        }
                        else break;
                    }
                }
            }

            //удалить позже
            //rotationIndex = maxColorsInBottle - bottleColors.Count + numberOfTopColorLayers;
        }

        private void UpdateColorsOnShader()
        {
            try
            {
                if (bottleColors.Count == 0)
                {
                    Debug.Log("bottleColors array is empty! Update Colors failed");
                    return;
                }
                for (var i = bottleColors.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        bottleMaskSR.material.SetColor("_Color" + (i + 1), bottleColors[i]);
                    }
                    catch (UnityException)
                    {
                        Debug.Log("Color" + (i + 1) + " wasn't set :P");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        //переписать индус код на нормальный
       

        private void RotationShaderComplete(float angleValue)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));
            
            bottleColors.RemoveRange(bottleColors.Count - numberOfColorsToTransfer,numberOfColorsToTransfer);
            UpdateTopColorValues();
        }

        private void RotateShader(float angleValue, float lastAngleValue)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmountValues[bottleColors.Count] > fillAmountCurve.Evaluate(angleValue))
            {
                bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

                bottleControllerRef.FillUp(fillAmountCurve.Evaluate(lastAngleValue) -
                                           fillAmountCurve.Evaluate(angleValue));
            }
        }

        private void RotateShaderBack(float angleValue)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty",
                scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        public void Initialize()
        {
            bottleMaskSR.material.SetFloat("_FillAmount", fillAmountValues[bottleColors.Count]);
            
            UpdateColorsOnShader();
            
            UpdateTopColorValues();
        }
        
        /*
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P) && justThisBottle && bottleColors.Count!=0)
            {
                if (bottleControllerRef.EnableToFillBottle(topColor))
                {
                    AddTopColorToAnother();
                    
                    ChooseRotationPointAndDirection(); 
                    CalculateRotationIndexToAnotherBottle();
                    StartCoroutine(RotateBottle());
                }
            }
        }
        */

        private void AddTopColorToAnother()
        {
            numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers,
                bottleControllerRef.maxColorsInBottle - bottleControllerRef.bottleColors.Count);
            for (var i = numberOfColorsToTransfer; i > 0; i--)
            {
                bottleControllerRef.bottleColors.Add(topColor);
            }
            bottleControllerRef.UpdateColorsOnShader();
            bottleControllerRef.UpdateTopColorValues();
        }

        private bool EnableToFillBottle(Color color)
        {
            return bottleColors.Count==0 || (color.Equals(topColor) && maxColorsInBottle - bottleColors.Count > 0);
        }

        public int CalculateRotationIndexToAnotherBottle()
        {
            return maxColorsInBottle - bottleColors.Count + numberOfColorsToTransfer;
        }

        private void FillUp(float fillAmountToAdd)
        {
            bottleMaskSR.material.SetFloat("_FillAmount",
                bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
        }

       

        public void OnBottleSelect()
        {
            
        }
    }
}

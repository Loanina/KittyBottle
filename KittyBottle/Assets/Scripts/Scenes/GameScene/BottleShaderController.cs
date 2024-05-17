using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Scenes.GameScene
{
    public class BottleShaderController : MonoBehaviour
    {
        private List<Color> _bottleColors;
        [SerializeField] private SpriteRenderer bottleMaskSR;
        

        [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
        [SerializeField] private AnimationCurve fillAmountCurve;
        

        [SerializeField] private float[] fillAmountValues;
        
        
        //потом вынести с отдельными кривыми и размерами бутылок для каждого
        [SerializeField] private int maxColorsInBottle = 4;
        
       
        private Color topColor;
        private int numberOfTopColorLayers = 1;

        //можно будет убрать
      
        [SerializeField] private bool justThisBottle = false;
        private int numberOfColorsToTransfer = 0;


        private void UpdateTopColorValues()
        {
            if (_bottleColors.Count == 0)
            {
                numberOfTopColorLayers = 0;
            }
            else
            {
                numberOfTopColorLayers = 1;
                topColor = _bottleColors[^1];
                
                if (_bottleColors.Count > 1)
                {
                    for (var i = _bottleColors.Count - 1; i > 0; i--)
                    {
                        if (_bottleColors[i].Equals(_bottleColors[i - 1]))
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
                if (_bottleColors.Count == 0)
                {
                    Debug.Log("bottleColors array is empty! Update Colors failed");
                    return;
                }
                for (var i = _bottleColors.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        bottleMaskSR.material.SetColor("_Color" + (i + 1), _bottleColors[i]);
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


        public void RotationShaderComplete(float angleValue)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));
            
            _bottleColors.RemoveRange(_bottleColors.Count - numberOfColorsToTransfer,numberOfColorsToTransfer);
            UpdateTopColorValues();
        }

        public void RotateShader(float angleValue, float lastAngleValue, Bottle targetBottle)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmountValues[_bottleColors.Count] > fillAmountCurve.Evaluate(angleValue))
            {
                bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

                targetBottle.FillUp(fillAmountCurve.Evaluate(lastAngleValue) -
                                           fillAmountCurve.Evaluate(angleValue));
            }
        }

        public void RotateShaderBack(float angleValue)
        {
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty",
                scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        public void Initialize(List<Color> bottleColors)
        {
            _bottleColors = bottleColors;
            bottleMaskSR.material.SetFloat("_FillAmount", fillAmountValues[_bottleColors.Count]);
            
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

        public void AddColor(int count, Color color)
        {
            numberOfColorsToTransfer = Mathf.Min(count,
                maxColorsInBottle - _bottleColors.Count);
            for (var i = numberOfColorsToTransfer; i > 0; i--)
            {
                _bottleColors.Add(color);
            }
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }

        private bool EnableToFillBottle(Color color)
        {
            return _bottleColors.Count==0 || (color.Equals(topColor) && maxColorsInBottle - _bottleColors.Count > 0);
        }

        public int CalculateRotationIndexToAnotherBottle()
        {
            return maxColorsInBottle - _bottleColors.Count + numberOfColorsToTransfer;
        }

        public void FillUp(float fillAmountToAdd)
        {
            bottleMaskSR.material.SetFloat("_FillAmount",
                bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
        }

       

        public void OnBottleSelect()
        {
            
        }
    }
}

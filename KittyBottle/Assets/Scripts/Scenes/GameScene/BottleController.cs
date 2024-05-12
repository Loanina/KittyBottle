using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scenes.GameScene
{
    public class BottleController : MonoBehaviour
    {
        [SerializeField] private List<Color> bottleColors;
        [SerializeField] private SpriteRenderer bottleMaskSR;
        [SerializeField] private float timeRotation = 1.0f;

        [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
        [SerializeField] private AnimationCurve fillAmountCurve;
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;

        [SerializeField] private float[] fillAmountValues;
        [SerializeField] private float[] rotationValues;
        
        //потом вынести с отдельными кривыми и размерами бутылок для каждого
        [SerializeField] private int maxColorsInBottle = 4;
        
        private int rotationIndex = 0;
        private Color topColor;
        private int numberOfTopColorLayers = 1;

        //можно будет убрать
        [SerializeField] private BottleController bottleControllerRef;
        [SerializeField] private bool justThisBottle = false;
        private int numberOfColorsToTransfer = 0;

        public Vector3 leftRotationPoint = new(-0.25f, 0.5f, 0.0f);
        public Vector3 rightRotationPoint = new(0.144f, 0.485f, 0.0f);
        private Vector3 chosenRotationPoint;

        private float directionMultiplier = 1.0f;

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
        private IEnumerator RotateBottle()
        {
            float time = 0;
            float lerpValue;
            float angleValue;
            float lastAngleValue = 0;
            
            Debug.Log("ROTATION INDEX "+ rotationIndex);
            
            while (time < timeRotation)
            {
                lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(0.0f,directionMultiplier * rotationValues[rotationIndex], lerpValue);
                
                //если rotation index = 0 устроить анимашку и запрет на поворот бутылки

               // transform.eulerAngles = new Vector3(0, 0, angleValue);
               
                transform.RotateAround(chosenRotationPoint, Vector3.forward, lastAngleValue - angleValue);
               
                bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
                
                if (fillAmountValues[bottleColors.Count] > fillAmountCurve.Evaluate(angleValue))
                {
                    bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

                    bottleControllerRef.FillUp(fillAmountCurve.Evaluate(lastAngleValue) -
                                               fillAmountCurve.Evaluate(angleValue));
                }

                time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
                lastAngleValue = angleValue;
                yield return new WaitForEndOfFrame();
            }

            angleValue = directionMultiplier * rotationValues[rotationIndex];
            //transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));
            
            bottleColors.RemoveRange(bottleColors.Count - numberOfColorsToTransfer,numberOfColorsToTransfer);
            UpdateTopColorValues();

            StartCoroutine(RotateBottleBack());
        }

        private IEnumerator RotateBottleBack()
        {
            float time = 0;
            float lerpValue;
            float angleValue;
            var lastAngleValue = directionMultiplier * rotationValues[rotationIndex];
            
            while (time < timeRotation)
            {
                lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(directionMultiplier * rotationValues[rotationIndex], 0.0f, lerpValue);

                //transform.eulerAngles = new Vector3(0, 0, angleValue);
                transform.RotateAround(chosenRotationPoint, Vector3.forward, lastAngleValue - angleValue);
                
                bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

                lastAngleValue = angleValue;
                
                time += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            angleValue = 0.0f;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            
            UpdateTopColorValues();
        }
        
        // Убрать всю хуйню как выстроишь архитектуру
        void Start()
        {
            bottleMaskSR.material.SetFloat("_FillAmount", fillAmountValues[bottleColors.Count]);
            
            UpdateColorsOnShader();
            
            UpdateTopColorValues();
        }

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

        private void CalculateRotationIndexToAnotherBottle()
        {
            rotationIndex = maxColorsInBottle - bottleColors.Count + numberOfColorsToTransfer;
        }

        private void FillUp(float fillAmountToAdd)
        {
            bottleMaskSR.material.SetFloat("_FillAmount",
                bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
        }

        private void ChooseRotationPointAndDirection()
        {
            if (transform.position.x > bottleControllerRef.transform.position.x)
            {
                chosenRotationPoint = leftRotationPoint;
                directionMultiplier = -1.0f;
                Debug.Log("left rotate");
            }
            else
            {
                chosenRotationPoint = rightRotationPoint;
                directionMultiplier = 1.0f;
                Debug.Log("right rotate");
            }
        }
    }
}

using System;
using System.Collections;
using ModestTree;
using UnityEngine;

namespace Scenes.GameScene
{
    public class BottleController : MonoBehaviour
    {
        [SerializeField] private Color[] bottleColors;
        [SerializeField] private SpriteRenderer bottleMaskSR;
        [SerializeField] private float timeRotation = 1.0f;

        [SerializeField] private AnimationCurve scaleAndRotationMultiplierCurve;
        [SerializeField] private AnimationCurve fillAmountCurve;
        [SerializeField] private AnimationCurve rotationSpeedMultiplier;

        [SerializeField] private float[] fillAmountValues;
        [SerializeField] private float[] rotationValues;
        
        private int rotationIndex = 0;

        //хз зачем отдельно создавать сериализованным и почему 4 цвета, это абсолютно не гибко, но для других значений
        //и бутылок необходимо будет узнавать свои значения для кривых, это я сделаю потом
        [Range(0,4)]
        [SerializeField] private int numberOfColorsInBottle = 4;
        
        //это я так понимаб чисто для дебага
        [SerializeField] private Color topColor;
        [SerializeField] private int numberOfTopColorLayers = 1;

        //numberOfColorsInBottle должно считаться автоматом а не ручками индус хуйню пишет, лучше сразу все переписывать
        //чем потом в болоте сидеть эти лестницы ломать
        void UpdateTopColorValues()
        { 
            if (numberOfColorsInBottle != 0)
            {
                numberOfTopColorLayers = 1;
                topColor = bottleColors[numberOfColorsInBottle - 1];

                if (numberOfColorsInBottle != 1)
                {
                    for (var i = numberOfColorsInBottle; i > 0; i--){
                        if (bottleColors[i - 1].Equals(bottleColors[i - 2]))
                        {
                            numberOfTopColorLayers++;
                        }
                        else break;
                    }
                }
            }
        }
        
        void UpdateColorsOnShader()
        {
            try
            {
                if (bottleColors.Length == 0)
                {
                    Debug.Log("bottleColors array is empty! Update Colors failed");
                    return;
                }
                for (var i = bottleColors.Length - 1; i >= 0; i--)
                {
                    try
                    {
                        bottleMaskSR.material.SetColor("_Color" + (i + 1), bottleColors[i]);
                        Debug.Log("Цвет " + (i + 1) + " установлен");
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
        IEnumerator RotateBottle()
        {
            float time = 0;
            float lerpValue;
            float angleValue;
            while (time < timeRotation)
            {
                lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(0.0f, 90.0f, lerpValue);

                transform.eulerAngles = new Vector3(0, 0, angleValue);
                bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
                
                if (fillAmountValues[numberOfColorsInBottle] > fillAmountCurve.Evaluate(angleValue))
                {
                    bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));
                }

                time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);

                yield return new WaitForEndOfFrame();
            }

            angleValue = 90.0f;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

            StartCoroutine(RotateBottleBack());
        }

        IEnumerator RotateBottleBack()
        {
            float time = 0;
            float lerpValue;
            float angleValue;
            while (time < timeRotation)
            {
                lerpValue = time / timeRotation;
                angleValue = Mathf.Lerp(90.0f, 0.0f, lerpValue);

                transform.eulerAngles = new Vector3(0, 0, angleValue);
                bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

                time += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            angleValue = 0.0f;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        }
        
        // Убрать всю хуйню как выстроишь архитектуру
        void Start()
        {
            bottleMaskSR.material.SetFloat("_FillAmount", fillAmountValues[numberOfColorsInBottle]);
            
            UpdateColorsOnShader();
            
            UpdateTopColorValues();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                StartCoroutine(RotateBottle());
            }
        }
    }
}

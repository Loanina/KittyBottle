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

        void UpdateColorsOnShader()
        { 
            foreach (var bottleColor in bottleColors)
            {
                try
                {
                    bottleMaskSR.material.SetColor("_Color" + (bottleColors.IndexOf(bottleColor)+1), bottleColor);
                }
                catch (UnityException)
                {
                    Debug.Log("Color" + (bottleColors.IndexOf(bottleColor)+1) +" wasn't set :P");
                }
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
                bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));

                time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);

                yield return new WaitForEndOfFrame();
            }

            angleValue = 90.0f;
            transform.eulerAngles = new Vector3(0, 0, angleValue);
            bottleMaskSR.material.SetFloat("_ScaleAndRotationMultiplyProperty", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
            bottleMaskSR.material.SetFloat("_FillAmount",fillAmountCurve.Evaluate(angleValue));
        }
        
        // Убрать всю хуйню как выстроишь архитектуру
        void Start()
        {
            UpdateColorsOnShader();
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

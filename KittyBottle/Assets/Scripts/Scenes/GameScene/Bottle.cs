using System;
using System.Collections;
using System.Collections.Generic;
using Scenes.GameScene;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private BottleController shaderController;
    [SerializeField] private float timeRotation = 1.0f;
    [SerializeField] private float[] rotationValues = {0.0f, 67.5f, 78.75f, 85.6f, 90.0f};
    [SerializeField] private AnimationCurve rotationSpeedMultiplier;
    
    private int rotationIndex = 0; //
    
    public Vector3 leftRotationPoint = new(-0.25f, 1.0f, 0.0f);
    public Vector3 rightRotationPoint = new(0.25f, 1.0f, 0.0f);
    private Vector3 chosenRotationPoint;
    
    private float directionMultiplier = 1.0f;

    private void Start()
    {
        shaderController.Initialize();
    }

    private IEnumerator RotateBottle()
    {
        float time = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = 0;
            
        while (time < timeRotation)
        {
            lerpValue = time / timeRotation;
            angleValue = Mathf.Lerp(0.0f,directionMultiplier * rotationValues[rotationIndex], lerpValue);
            
            transform.RotateAround(chosenRotationPoint, Vector3.forward, lastAngleValue - angleValue);
               
           shaderController.RotateShader(angleValue, lastAngleValue);

            time += Time.deltaTime * rotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }

        angleValue = directionMultiplier * rotationValues[rotationIndex];
        shaderController.RotationShaderComplete(angleValue);

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
                
            shaderController.RotateShaderBack(angleValue);

            lastAngleValue = angleValue;
                
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        angleValue = 0.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        shaderController.RotateShaderBack(angleValue);
            
       // UpdateTopColorValues(); наврн не нужно
    }
    
   /* private void ChooseRotationPointAndDirection()
    {
        if (transform.position.x > bottleControllerRef.transform.position.x)
        {
            chosenRotationPoint = leftRotationPoint + transform.position;
            directionMultiplier = -1.0f;
        }
        else
        {
            chosenRotationPoint = rightRotationPoint + transform.position;
            directionMultiplier = 1.0f;
        }
    }*/
}

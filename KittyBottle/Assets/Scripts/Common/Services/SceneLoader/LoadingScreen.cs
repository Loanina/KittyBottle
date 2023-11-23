using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private float progressSpeed = 3;
    private float _targetProgress;

    private void Update()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, _targetProgress, progressSpeed * Time.deltaTime);
    }

    public void UpdateTargetProgress(float newTarget)
    {
        _targetProgress = newTarget;
    }

    public void ResetValues()
    {
        progressBar.value = 0;
        _targetProgress = 0;
    }
}
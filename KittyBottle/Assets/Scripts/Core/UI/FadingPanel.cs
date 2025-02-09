using System;
using DG.Tweening;
using UnityEngine;

public class FadingPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    public Tween FadeTween { get; private set; }

    private void OnDestroy()
    {
        CheckTween();
    }

    public void Refresh()
    {
        CheckTween();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void FadeIn(float duration, Ease ease, float delay = 0, Action onComplete = null)
    {
        FadeTween = canvasGroup.DOFade(1, duration).SetDelay(delay).SetEase(ease);
        FadeTween.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration, Ease ease, float delay = 0, Action onComplete = null)
    {
        FadeTween = canvasGroup.DOFade(0, duration).SetDelay(delay).SetEase(ease);
        FadeTween.OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            onComplete?.Invoke();
        });
    }
    
    private void CheckTween()
    {
        if (FadeTween != null)
        {
            FadeTween.Kill();
        }
    }

}

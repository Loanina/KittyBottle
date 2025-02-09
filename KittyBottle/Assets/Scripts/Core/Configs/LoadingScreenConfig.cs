using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "UI/LoadingScreen/LoadingScreenConfig")]
public class LoadingScreenConfig : ScriptableObject
{
    [SerializeField] private int delayToUpdateProgressbarMS = 1000;
    [SerializeField] private float fadeinTime = 1;
    [SerializeField] private float fadeinDelay = 0;
    [SerializeField] private float fadeOutTime = 1;
    [SerializeField] private float fadeOutDelay = 0;
    [SerializeField] private Ease fadingEase = Ease.Linear;

    public int DelayToUpdateProgressbarMS => delayToUpdateProgressbarMS;
    public float FadeinTime => fadeinTime;
    public float FadeinDelay => fadeinDelay;
    public float FadeOutTime => fadeOutTime;
    public float FadeOutDelay => fadeOutDelay;
    public Ease FadingEase => fadingEase;
}

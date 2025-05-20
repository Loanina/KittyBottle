using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.GameScene.Reward.Animation
{
    public class RewardBagAnimator
    {
        private readonly RewardBagAnimationConfig config;
        
        public RewardBagAnimator(RewardBagAnimationConfig config)
        {
            this.config = config;
        }

        public void PlayAppear(Image backgroundImage, RectTransform bag)
        {
            var bgColor = backgroundImage.color;
            bgColor.a = config.minAlpha;
            backgroundImage.color = bgColor;

            backgroundImage.DOFade(config.maxAlpha, config.backgroundAppearDuration)
                .SetEase(Ease.OutCubic);

            bag.localScale = Vector3.zero;
            bag.DOScale(Vector3.one, config.bagAppearDuration).SetEase(Ease.OutBack);
        }
        
        public void PlayPickup(RectTransform bag, System.Action onComplete = null)
        {
            bag.DOKill();
            var seq = DOTween.Sequence();
            seq.Append(bag.DOScale(config.pickupScale * Vector3.one, config.pickupDuration / 2).SetEase(Ease.OutQuad));
            seq.Append(bag.DOScale(Vector3.one, config.pickupDuration / 2).SetEase(Ease.InQuad)).
                OnComplete(() => onComplete?.Invoke());
        }

        public void PlayDisappear(Image backgroundImage, RectTransform bag, System.Action onComplete = null)
        {
            backgroundImage.DOFade(config.minAlpha, config.backgroundDisappearDuration)
                .SetEase(Ease.InCubic);
        
            bag.DOScale(Vector3.zero, config.bagDisappearDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
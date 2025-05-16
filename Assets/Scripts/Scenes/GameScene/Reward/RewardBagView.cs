using System;
using Scenes.GameScene.Reward.Animation;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scenes.GameScene.Reward
{
    public class RewardBagView : MonoBehaviour
    {
        
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform bag;
        [SerializeField] private RectTransform itemsRoot;
        private RewardBagAnimator animator;
        private RewardItemFactory itemFactory;
        private RewardConfig config;

        [Inject]
        public void Construct(RewardBagAnimator bagAnimator, RewardItemFactory itemFactory, RewardConfig config)
        {
            animator = bagAnimator;
            this.itemFactory = itemFactory;
            this.config = config;
        }
        
        public event Action onClaimed;

        public void Setup(RewardData data)
        {
            foreach (var entry in data.GetAllRewards())
            {
                var item = itemFactory.Create();
               item.transform.SetParent(itemsRoot, false);
               item.Setup(config.GetSprite(entry.type), entry.amount);
            }

            Debug.Log((animator == null) + " animator is null ");
            if (animator != null) animator.PlayAppear(backgroundImage, bag);
        }
    }
}
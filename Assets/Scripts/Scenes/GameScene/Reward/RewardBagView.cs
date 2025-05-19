    using System;
    using Core.InputSystem;
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
            [SerializeField] private Button interactableButton;
            private RewardBagAnimator animator;
            private RewardItemFactory itemFactory;
            private RewardConfig config;
            public event Action OnClaimed;
            
            [Inject]
            public void Construct(RewardBagAnimator bagAnimator, RewardItemFactory itemFactory, RewardConfig config)
            {
                animator = bagAnimator;
                this.itemFactory = itemFactory;
                this.config = config;
            }

            private void OnEnable()
            {
                interactableButton.onClick.AddListener(OnClick);
            }

            private void OnDisable()
            {
                interactableButton.onClick.RemoveListener(OnClick);
            }

            private void Show() => animator.PlayAppear(backgroundImage, bag);
            public void Hide(Action onComplete = null) => animator.PlayDisappear(backgroundImage, bag, onComplete);
            public void OnClick()
            {
                interactableButton.interactable = false;
                animator.PlayPickup(bag);
                OnClaimed?.Invoke();
            }

            public void Setup(RewardData data)
            {
                foreach (var entry in data.GetAllRewards())
                {
                    var item = itemFactory.Create();
                   item.transform.SetParent(itemsRoot, false);
                   item.Setup(config.GetSprite(entry.type), entry.amount);
                }
                Show();
            }
        }
    }
using System;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Reward
{
    public class RewardService
    {
        private readonly RewardFactory factory;
        private readonly RectTransform rewardRoot;
        private readonly RewardInventoryService inventoryService;

        [Inject]
        public RewardService(RewardFactory factory, RectTransform rewardRoot, RewardInventoryService inventoryService)
        {
            this.factory = factory;
            this.rewardRoot = rewardRoot;
            this.inventoryService = inventoryService;
        }

        public void ShowRewards(RewardData rewardData, Action OnComplete)
        {
            var bagView = factory.Create();
            bagView.Setup(rewardData);
            bagView.transform.SetParent(rewardRoot, false);
            bagView.OnClaimed += () =>
            {
                GrantRewards(rewardData);
                OnComplete?.Invoke();
            };
        }

        private void GrantRewards(RewardData data)
        {
            foreach (var entry in data.GetAllRewards())
            {
                inventoryService.Add(entry.type, entry.amount);
            }
        }
    }
}
using System;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene.Reward
{
    public class RewardService
    {
        private readonly RewardFactory factory;
        private readonly RectTransform rewardRoot;

        [Inject]
        public RewardService(RewardFactory factory, RectTransform rewardRoot)
        {
            this.factory = factory;
            this.rewardRoot = rewardRoot;
        }

        public void ShowRewards(RewardData rewardData, Action OnComplete)
        {
            var bagView = factory.Create();
            bagView.Setup(rewardData);
            bagView.transform.SetParent(rewardRoot, false);
            bagView.OnClaimed += () => OnComplete?.Invoke();
        }
        
        public void GrantRewards(RewardData data)
        {
            foreach (var entry in data.GetAllRewards())
            {
                // зачесть монетки, подсказки, билетики
            }
        }
    }
}
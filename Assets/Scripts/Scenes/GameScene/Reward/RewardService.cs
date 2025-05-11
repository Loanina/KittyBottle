using System;
using UnityEngine;

namespace Scenes.GameScene.Reward
{
    public class RewardService
    {
        private readonly RewardConfig config;
        private readonly Transform uiParent;

        public RewardService(RewardConfig config, Transform uiParent)
        {
            this.config = config;
            this.uiParent = uiParent;
        }

        public void ShowRewards(RewardData rewardData, Action OnComplete)
        {
            var bagView = GameObject.Instantiate(config.bagSource, uiParent);
            bagView.Setup(rewardData, config);
            bagView.onClaimed += () => OnComplete?.Invoke();
        }
    }
}
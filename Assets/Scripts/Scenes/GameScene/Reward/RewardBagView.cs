using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.GameScene.Reward
{
    public class RewardBagView : MonoBehaviour
    {
        [SerializeField] private Button clickButton;
        [SerializeField] private Transform gridRoot;

        public event Action onClaimed;

        public void Setup(RewardData data, RewardConfig config)
        {
            foreach (var entry in data.GetAllRewards())
            {
                var item = Instantiate(config.itemSource, gridRoot);
                item.Setup(config.GetSprite(entry.type) , entry.amount);
            }
        }
    }
}
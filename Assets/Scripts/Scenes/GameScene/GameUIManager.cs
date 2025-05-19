using Scenes.GameScene.Reward;
using TMPro;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene
{
    public class GameUIManager: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI hintCount;
        [SerializeField] private TextMeshProUGUI undoCount;
        private RewardInventoryService inventoryService;
        
        [Inject]
        public void Construct(RewardInventoryService inventory)
        {
            inventoryService = inventory;
            inventoryService.OnRewardChanged += OnRewardChanged;
            OnRewardChanged(RewardType.Coins, inventory.GetCoins());
            OnRewardChanged(RewardType.Hints, inventory.GetHints());
            OnRewardChanged(RewardType.Undo, inventory.GetUndo());
        }
        
        private void OnRewardChanged(RewardType type, int value)
        {
            switch (type)
            {
                case RewardType.Coins: SetMoney(value); break;
                case RewardType.Hints: SetHint(value); break;
                case RewardType.Undo:  SetUndo(value); break;
            }
        }

        private void SetMoney(int count) => moneyText.text = count.ToString();
        private void SetHint(int count) => hintCount.text = count.ToString();
        private void SetUndo(int count) => undoCount.text = count.ToString();
    }
}
using System;
using Core.SavingSystem;
using IInitializable = Zenject.IInitializable;

namespace Scenes.GameScene
{
    public class MoneyManager: IInitializable
    {
        private int coins;
        private event Action<int> OnMoneyUpdate;
        private PlayerProgressService playerProgressService;
        private int coinsPerLevel;

        public MoneyManager(PlayerProgressService playerProgressService, int coinsPerLevel)
        {
            this.playerProgressService = playerProgressService;
            this.coinsPerLevel = coinsPerLevel;
        }

        public void Initialize()
        {
            coins = playerProgressService.GetMoneyCount();
            UpdateMoneyValue();
        }

        public void AddCoins(int count)
        {
            coins += count;
            UpdateMoneyValue();
            playerProgressService.SetMoney(coins);
        }

        public void AddCoinsForLevel() => AddCoins(coinsPerLevel);

        private void UpdateMoneyValue() => OnMoneyUpdate?.Invoke(coins);
    }
}
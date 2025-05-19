using System;
using System.Collections.Generic;
using Core.SavingSystem;
using Zenject;

namespace Scenes.GameScene.Reward
{
    public class RewardInventoryService
    {
        private readonly PlayerProgressService progress;
        private readonly Dictionary<RewardType, int> rewards = new();
        public event Action<RewardType, int> OnRewardChanged;

        [Inject]
        public RewardInventoryService(PlayerProgressService progress)
        {
            this.progress = progress;
            LoadAll();
        }

        private void LoadAll()
        {
            foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
                rewards[type] = progress.GetReward(type);
        }

        private void Save(RewardType type)
        {
            progress.SetReward(type, rewards[type]);
        }

        public int Get(RewardType type) => rewards.TryGetValue(type, out var value) ? value : 0;

        public void Add(RewardType type, int amount)
        {
            if (!rewards.ContainsKey(type)) rewards[type] = 0;
            rewards[type] += amount;
            Save(type);
            OnRewardChanged?.Invoke(type, rewards[type]);
        }

        public void Set(RewardType type, int amount)
        {
            rewards[type] = amount;
            Save(type);
            OnRewardChanged?.Invoke(type, amount);
        }
        
        public int GetCoins() => Get(RewardType.Coins);
        public void AddCoins(int amount) => Add(RewardType.Coins, amount);
        public void SetCoins(int amount) => Set(RewardType.Coins, amount);

        public int GetHints() => Get(RewardType.Hints);
        public void AddHints(int amount) => Add(RewardType.Hints, amount);
        public void SetHints(int amount) => Set(RewardType.Hints, amount);

        public int GetUndo() => Get(RewardType.Undo);
        public void AddUndo(int amount) => Add(RewardType.Undo, amount);
        public void SetUndo(int amount) => Set(RewardType.Undo, amount);

        public int GetTickets() => Get(RewardType.Tickets);
        public void AddTickets(int amount) => Add(RewardType.Tickets, amount);
        public void SetTickets(int amount) => Set(RewardType.Tickets, amount);
    }
}

using System;
using Common.DataManagement;
using Scenes.GameScene.Reward;
using Zenject;

namespace Core.SavingSystem
{
    public class PlayerProgressService
    {
        private readonly ISaveSystem<PlayerData> saveSystem;

        [Inject]
        public PlayerProgressService(ISaveSystem<PlayerData> saveSystem)
        {
            this.saveSystem = saveSystem;
        }
    
        public int GetLastCompletedLevelID()
        {
            return saveSystem.Load().lastLevelID;
        }

        public void SetLastCompletedLevel(int levelIndex)
        {
            var data = saveSystem.Load();
            data.lastLevelID = levelIndex;
            saveSystem.Save(data);
        }

        public int GetReward(RewardType type)
        {
            return type switch
            {
                RewardType.Coins => saveSystem.Load().coins,
                RewardType.Hints => saveSystem.Load().hints,
                RewardType.Undo => saveSystem.Load().undo,
                RewardType.Tickets => saveSystem.Load().tickets,
                _ => throw new ArgumentException(nameof(type), "Reward type")
            };
        }
        
        public void SetReward(RewardType type, int count)
        {
            var data = saveSystem.Load();
            switch (type)
            {
                case RewardType.Coins:
                    data.coins = count;
                    break;
                case RewardType.Hints:
                    data.hints = count;
                    break;
                case RewardType.Undo:
                    data.undo = count;
                    break;
                case RewardType.Tickets:
                    data.tickets = count;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            saveSystem.Save(data);
        }
    }
}
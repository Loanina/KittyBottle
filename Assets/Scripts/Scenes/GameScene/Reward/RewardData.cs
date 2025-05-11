using System.Collections.Generic;

namespace Scenes.GameScene.Reward
{
    [System.Serializable]
    public class RewardData
    {
        public int coins;
        public int hints;
        public int undo;
        public int tickets;
        
        public List<RewardEntry> GetAllRewards()
        {
            var list = new List<RewardEntry>();
            if (coins > 0) list.Add(new RewardEntry { type = RewardType.Coins, amount = coins });
            if (hints > 0) list.Add(new RewardEntry { type = RewardType.Hints, amount = hints });
            if (undo > 0) list.Add(new RewardEntry { type = RewardType.Undo, amount = undo });
            if (tickets > 0) list.Add(new RewardEntry { type = RewardType.Tickets, amount = tickets });
            return list;
        }
    }
    
    public class RewardEntry
    {
        public RewardType type;
        public int amount;
    }
    
    public enum RewardType { Coins, Hints, Undo, Tickets }
}
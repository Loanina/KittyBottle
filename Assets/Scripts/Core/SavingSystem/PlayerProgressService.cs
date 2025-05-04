using Common.DataManagement;
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

        public int GetMoneyCount() => saveSystem.Load().coins;
        public void SetMoney(int coins)
        {
            var data = saveSystem.Load();
            data.coins = coins;
            saveSystem.Save(data);
        }
    }
}
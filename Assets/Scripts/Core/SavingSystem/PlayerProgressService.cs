using Common.DataManagement;
using Zenject;

namespace Core.SavingSystem
{
    public class PlayerProgressService
    {
        private readonly ISaveSystem<PlayerData> saveSystem;
        private readonly int coinsPerLevel;

        [Inject]
        public PlayerProgressService(ISaveSystem<PlayerData> saveSystem, int coinsPerLevel)
        {
            this.saveSystem = saveSystem;
            this.coinsPerLevel = coinsPerLevel;
        }
    
        public int GetLastCompletedLevel()
        {
            return saveSystem.Load().lastLevelID;
        }

        public void UpdateProgress(int levelIndex)
        {
            var data = saveSystem.Load();
            data.coins += coinsPerLevel;
            data.lastLevelID = levelIndex;
            saveSystem.Save(data);
        }
    }
}
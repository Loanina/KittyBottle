namespace Common.DataManagement
{
    [System.Serializable]
    public class PlayerData
    {
        public int lastLevelID;
        public int coins;

        public PlayerData(int lastLevelID, int coins)
        {
            this.lastLevelID = lastLevelID;
            this.coins = coins;
        }

        public PlayerData() : this(0, 0) {}
    }
}

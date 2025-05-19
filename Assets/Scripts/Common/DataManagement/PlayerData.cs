namespace Common.DataManagement
{
    [System.Serializable]
    public class PlayerData
    {
        public int lastLevelID;
        public int coins;
        public int hints;
        public int undo;
        public int tickets;

        public PlayerData(int lastLevelID, int coins, int hints, int undo, int tickets)
        {
            this.lastLevelID = lastLevelID;
            this.coins = coins;
            this.hints = hints;
            this.undo = undo;
            this.tickets = tickets;
        }

        public PlayerData() : this(0, 0, 0, 0, 0) {}
    }
}

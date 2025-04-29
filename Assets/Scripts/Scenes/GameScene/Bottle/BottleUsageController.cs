namespace Scenes.GameScene.Bottle
{
    public class BottleUsageController
    {
        public bool InUse { get; private set; }
        public int UsesCount { get; private set; }

        public void StartUse() => InUse = true;
        public void EndUse() => InUse = false;

        public void IncreaseUsageCount() => UsesCount++;
        public void DecreaseUsageCount()
        {
            if (UsesCount > 0) UsesCount--;
        }
    }

}
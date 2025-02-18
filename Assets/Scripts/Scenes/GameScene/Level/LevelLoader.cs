namespace Scenes.GameScene.Level
{
    public class LevelLoader 
    {
        public LevelData LoadLevel(int index, LevelCollection collection)
        {
            if (index >= 0 && index < collection.levels.Count)
            {
                return collection.levels[index];
            }
            return null;
        }
    }
}
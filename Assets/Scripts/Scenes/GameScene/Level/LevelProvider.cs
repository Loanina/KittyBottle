using System;

namespace Scenes.GameScene.Level
{
    public class LevelProvider
    {
        private readonly LevelCollection levelCollection;

        public LevelProvider(LevelCollection levelCollection)
        {
            this.levelCollection = levelCollection;
        }

        public LevelData GetLevel(int index)
        {
            if (!HasLevel(index))
                throw new ArgumentOutOfRangeException(nameof(index), $"Level {index} not found");
        
            return levelCollection.levels[index];
        }

        public bool HasLevel(int index)
        {
            return index >= 0 && index < levelCollection.levels.Count;
        }
    }
}
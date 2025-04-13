using System.Collections.Generic;

namespace Scenes.GameScene.Level
{
    public class LevelCompletionChecker
    {
        public bool CheckIfLevelComplete(List<Bottle.Bottle> bottles)
        {
            foreach (var bottle in bottles)
            {
                if (!(bottle.IsFull() || bottle.IsEmpty()))
                    return false;
            }
            return true;
        }
    }
}
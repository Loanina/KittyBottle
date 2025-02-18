using System.Collections.Generic;
using Scenes.GameScene.Bottle;

namespace Core.Hints
{
    public interface IMoveStrategy
    {
        (int, int) FindBestMove(IReadOnlyList<BottleState> bottles);
    }
}
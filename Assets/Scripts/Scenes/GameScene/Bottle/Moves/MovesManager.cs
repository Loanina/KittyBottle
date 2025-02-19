using System.Collections.Generic;

namespace Scenes.GameScene.Bottle.Moves
{
    public class MovesManager
    {
        private Stack<Move> moves;

        public void AddMove(int from, int to, int countOfColorToTransfer)
        {
            moves.Push(new Move(from, to, countOfColorToTransfer));
        }

        public Move PopLastMove()
        {
            return moves.Pop();
        }

        public void ClearMoves()
        {
            moves.Clear();
        }
    }
}

using System.Collections.Generic;

namespace Core.Hints.Moves
{
    public class MoveHistory
    {
        private readonly Stack<Move> moves = new Stack<Move>();
        public bool HasMoves => moves.Count > 0;
        public void RecordMove(int from, int to, int countOfColorToTransfer)
        {
            moves.Push(new Move(from, to, countOfColorToTransfer));
        }

        public bool TryUndoLastMove(out Move move)
        {
            if (moves.Count == 0)
            {
                move = default;
                return false;
            }

            move = moves.Pop();
            return true;
        }

        public void ClearMoves()
        {
            moves.Clear();
        }
    }
}

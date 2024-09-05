using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class MovesManager : MonoBehaviour
    {
        private BottlesController bottlesController;
        private Stack<Move> moves;

        public void AddMove(int from, int to, int countOfColorToTransfer)
        {
            moves.Push(new Move(from, to, countOfColorToTransfer));
        }

        public void ReturnMove()
        {
            var lastMove = moves.Pop();
            bottlesController.TransferColorWithoutAnimation(lastMove.from, lastMove.to, lastMove.countOfColorToTransfer);
        }

        public void ClearMoves()
        {
            moves.Clear();
        }
    }
}

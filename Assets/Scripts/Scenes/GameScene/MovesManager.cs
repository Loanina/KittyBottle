using System.Collections.Generic;
using Scenes.GameScene.Bottle;
using UnityEngine;
using Zenject;

namespace Scenes.GameScene
{
    public class MovesManager
    {
        private readonly BottlesController bottlesController;
        private Stack<Move> moves;

        [Inject]
        public MovesManager(BottlesController bottlesController)
        {
            this.bottlesController = bottlesController;
        }

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

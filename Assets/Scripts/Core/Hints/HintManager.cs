using System;
using Core.Hints.Moves;
using UnityEngine;
using Zenject;

namespace Core.Hints
{
    public class HintManager : MonoBehaviour
    {
        private MoveHistory moveHistory;
        
        public event Action OnRestartRequested;
        public event Action OnBestMoveRequested;
        public event Action<Move> OnReturnMove;
        
        [Inject]
        public void Construct(MoveHistory moveHistory)
        {
            this.moveHistory = moveHistory;
        }

        public void OnRestartLevel()
        {
            OnRestartRequested?.Invoke();
        }

        public void OnGetBestMove()
        {
            OnBestMoveRequested?.Invoke();
        }

        public void AddMove(int from, int to, int countOfColorToTransfer)
        {
            moveHistory.RecordMove(from, to, countOfColorToTransfer);
        }

        public void ReturnMove()
        {
            if (moveHistory.TryUndoLastMove(out var lastMove))
            {
                OnReturnMove?.Invoke(lastMove);
            }
            else Debug.Log("There is no last move");
        }
    }
}

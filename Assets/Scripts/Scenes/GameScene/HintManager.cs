using System;
using UnityEngine;

namespace Scenes.GameScene
{
    public class HintManager : MonoBehaviour
    {
        public event Action OnRestartRequested;
        public event Action OnBestMoveRequested;
        public event Action OnReturnRequested; 

        public void OnRestartLevel()
        {
            OnRestartRequested?.Invoke();
        }

        public void OnGetBestMove()
        {
            OnBestMoveRequested?.Invoke();
        }

        public void OnReturnMove()
        {
            OnReturnRequested?.Invoke();
        }
        
        
    }
}

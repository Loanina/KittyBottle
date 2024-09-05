using System;
using UnityEngine;

namespace Scenes.GameScene
{
    public class HintManager : MonoBehaviour
    {
        public event Action OnRestartEvent;
        public event Action OnGetBestMoveEvent;
        public event Action OnReturnMoveEvent; 

        public void OnRestartLevel()
        {
            OnRestartEvent?.Invoke();
        }

        public void OnGetBestMove()
        {
            OnGetBestMoveEvent?.Invoke();
        }

        public void OnReturnMove()
        {
            OnReturnMoveEvent?.Invoke();
        }
        
        
    }
}

using System;
using UnityEngine;

namespace Scenes.GameScene
{
    public class HintManager : MonoBehaviour
    {
        public event Action OnRestartEvent;
        public event Action OnGetBestMoveEvent; 

        public void OnRestartLevel()
        {
            OnRestartEvent?.Invoke();
        }

        public void OnGetBestMove()
        {
            OnGetBestMoveEvent?.Invoke();
        }
        
        
    }
}

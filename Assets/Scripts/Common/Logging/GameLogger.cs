using UnityEngine;

namespace Common.Logging
{
    public class GameLogger : IGameLogger
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }
        public void LogError(string message) => Debug.LogError(message);
    }
}
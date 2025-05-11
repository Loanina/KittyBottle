using System.Collections.Generic;
using Scenes.GameScene.Reward;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level data")]
    public class LevelData : SerializedScriptableObject
    {
        [ShowInInspector]
        public List<List<int>> level;
        
        public RewardData reward;
    }
}

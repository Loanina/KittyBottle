using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level data")]
    public class LevelData : SerializedScriptableObject
    {
        [ShowInInspector]
        public List<List<int>> level;
    }
}

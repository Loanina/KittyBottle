using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scenes.GameScene.Level
{
    [CreateAssetMenu(fileName = "LevelCollection", menuName = "Game/Level collection")]
    public class LevelCollection : SerializedScriptableObject
    {
        public List<LevelData> levels;
    }
}

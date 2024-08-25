using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Scenes.GameScene.Bottle
{
    [CreateAssetMenu(fileName = "LayoutSettings", menuName = "Game/Layout Settings")]
    public class LayoutSettings : SerializedScriptableObject
    {
        [ShowInInspector]
        public Dictionary<int, List<Vector3>> layoutSettings;
        public List<Vector3> GetLayout(int bottlesCount)
        {
            return layoutSettings[bottlesCount];
        }
    }
}

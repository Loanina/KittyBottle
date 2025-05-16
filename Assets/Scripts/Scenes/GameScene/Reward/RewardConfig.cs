using System.Collections.Generic;
using Scenes.GameScene.Reward.Animation;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Scenes.GameScene.Reward
{
    [CreateAssetMenu (fileName = "RewardConfig", menuName = "Game/Reward Config")]
    public class RewardConfig : SerializedScriptableObject
    {
        public RewardBagAnimationConfig animationConfig;
        
        [OdinSerialize]
        private Dictionary<RewardType, Texture2D> rewardTextures = new();
        private Dictionary<RewardType, Sprite> spriteCache;

        public Sprite GetSprite(RewardType type)
        {
            if (spriteCache == null)
                BuildSpriteCache();

            return spriteCache.TryGetValue(type, out var sprite) ? sprite : null;
        }

        private void BuildSpriteCache()
        {
            spriteCache = new Dictionary<RewardType, Sprite>();

            foreach (var kvp in rewardTextures)
            {
                var texture = kvp.Value;
                if (texture == null) continue;

                var sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
                spriteCache[kvp.Key] = sprite;
            }
        }
    }
}
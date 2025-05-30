﻿using UnityEngine;

namespace Scenes.GameScene.Reward.Animation
{
    [CreateAssetMenu(fileName = "RewardBagAnimationConfig", menuName = "Game/Animations/Reward Bag Animation Config")]
    public class RewardBagAnimationConfig : ScriptableObject
    {
        [Header("Background")]
        [Range(0,10)] public float backgroundAppearDuration = 0.3f;
        [Range(0,10)] public float backgroundDisappearDuration = 0.3f;
        [Range(0, 1)] public float maxAlpha = 1;
        [Range(0, 1)] public float minAlpha = 0;

        [Header("Bag Appear")]
        [Range(0,10)] public float bagAppearDuration = 0.4f;

        [Header("Bag Pickup")]
        [Range(0.1f,5f)] public float pickupScale = 1.1f;
        [Range(0,10)] public float pickupDuration = 0.2f;

        [Header("Bag Disappear")]
        [Range(0,10)] public float bagDisappearDuration = 0.25f;
        
        [Header("Pulse Animation")]
        [Range(0.1f,5f)] public float maxPulseScale = 1.1f;
        [Range(0.1f,5f)] public float minPulseScale = 0.9f;
        [Range(0,10)] public float pulseDuration = 1.5f;
    }
} 
using UnityEngine;
using Zenject;
using Scenes.GameScene.Reward;
using Scenes.GameScene.Reward.Animation;

namespace Core.Installers
{
    [CreateAssetMenu(fileName = "RewardProjectInstaller", menuName = "Game/Installers/RewardProjectInstaller")]
    public class RewardProjectInstaller : ScriptableObjectInstaller<RewardProjectInstaller>
    {
        [SerializeField] private RewardConfig rewardConfig;
        [SerializeField] private RewardItemView itemPrefab;
        [SerializeField] private RewardBagView bagPrefab;

        public override void InstallBindings()
        {
            Container.Bind<RewardConfig>().FromInstance(rewardConfig).AsSingle();
            Container.BindFactory<RewardItemView, RewardItemFactory>()
                .FromComponentInNewPrefab(itemPrefab)
                .UnderTransformGroup("RewardItems");
            Container.BindFactory<RewardBagView, RewardFactory>()
                .FromComponentInNewPrefab(bagPrefab)
                .UnderTransformGroup("RewardBags");
            Container.Bind<RewardBagAnimator>().AsSingle()
                .WithArguments(rewardConfig.animationConfig);
        }
    }
}
using UnityEngine;

namespace XIFramework.GameFramework
{
// 世界特定配置
    [CreateAssetMenu(fileName = "WorldFeatureConfig", menuName = "Game/Feature Configs/World Specific")]
    public class WorldFeatureConfig : XIGameFeatureConfig
    {
        public string targetWorldName;
        [Header("World Specific Features")] public XIGameFeature worldPhysicsFeature;
        public XIGameFeature aiSystemFeature;
        public override void ApplyConfig(XIGameWorld world)
        {
            // 只应用于目标世界
            if (world.Context.Name != targetWorldName) return;
            base.ApplyConfig(world);
            if (worldPhysicsFeature != null)
            {
                var feature = Instantiate(worldPhysicsFeature);
                world.FeatureManager.LoadFeature(feature, FeatureLoadMode.InitializeWithWorld);
            }
            if (aiSystemFeature != null)
            {
                var feature = Instantiate(aiSystemFeature);
                world.FeatureManager.LoadFeature(feature, FeatureLoadMode.InitializeWithWorld);
            }
        }
    }
}
using UnityEngine;

namespace XIFramework.GameFramework
{
// 平台特定配置 - 文件名: PlatformFeatureConfig.cs
    [CreateAssetMenu(fileName = "PlatformFeatureConfig", menuName = "Game/Feature Configs/Platform Specific")]
    public class PlatformFeatureConfig : XIGameFeatureConfig
    {
        public RuntimePlatform targetPlatform;
    
        public override void ApplyConfig(XIGameWorld world)
        {
            if (Application.platform != targetPlatform) return;
        
            foreach (var entry in featureEntries)
            {
                if (!entry.enabled) continue;
            
                var feature = Instantiate(entry.feature);
                feature.Scope = XIGameFeature.FeatureScope.World;
            
                foreach (var param in entry.parameters)
                {
                    feature.SetParameter(param.key, param.value);
                }
            
                world.FeatureManager.LoadFeature(feature, entry.loadMode);
            }
        }
    }
}
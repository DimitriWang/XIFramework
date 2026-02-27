using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界特性配置
    /// </summary>
    [CreateAssetMenu(fileName = "WorldFeatureConfig", menuName = "GameFramework/Feature Configs/World")]
    public class WorldFeatureConfig : FeatureConfig
    {
        [Header("世界特定设置")]
        public string TargetWorldName;
        public bool ApplyToWorldChildren = false;
        
        [Header("世界功能")]
        public bool EnablePhysics = true;
        public bool EnableAI = true;
        public bool EnableNetworking = false;
        
        [Header("性能调整")]
        public float WorldTimeScale = 1.0f;
        public int MaxActiveActors = 100;
        
        public override void ApplyConfig(IGameWorld world)
        {
            if (!IsEnabled)
                return;
                
            if (world.Name != TargetWorldName && !ApplyToWorldChildren)
                return;
                
            Debug.Log($"[WorldFeatureConfig] Applying world config to: {world.Name}");
            
            ApplyWorldSettings(world);
        }
        
        private void ApplyWorldSettings(IGameWorld world)
        {
            // 时间缩放设置
            // Time.timeScale = WorldTimeScale;
            
            Debug.Log($"[WorldFeatureConfig] World time scale: {WorldTimeScale}");
            Debug.Log($"[WorldFeatureConfig] Physics enabled: {EnablePhysics}, AI enabled: {EnableAI}");
        }
    }
}
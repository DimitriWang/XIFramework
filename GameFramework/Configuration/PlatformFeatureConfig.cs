using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 平台特性配置
    /// </summary>
    [CreateAssetMenu(fileName = "PlatformFeatureConfig", menuName = "GameFramework/Feature Configs/Platform")]
    public class PlatformFeatureConfig : FeatureConfig
    {
        [Header("平台特定设置")]
        public RuntimePlatform TargetPlatform;
        public bool OverrideDefaultSettings = true;
        
        [Header("性能设置")]
        public int PlatformTargetFrameRate = 30;
        public bool EnablePlatformOptimizations = true;
        
        [Header("平台功能")]
        public bool EnableVibration = true;
        public bool EnableGyroscope = false;
        
        public override void ApplyConfig(IGameWorld world)
        {
            if (!IsEnabled)
                return;
                
            if (Application.platform != TargetPlatform)
                return;
                
            Debug.Log($"[PlatformFeatureConfig] Applying platform config for {TargetPlatform} to world: {world.Name}");
            
            ApplyPlatformSettings();
        }
        
        private void ApplyPlatformSettings()
        {
            if (OverrideDefaultSettings)
            {
                Application.targetFrameRate = PlatformTargetFrameRate;
            }
            
            Debug.Log($"[PlatformFeatureConfig] Platform target frame rate: {PlatformTargetFrameRate}");
            Debug.Log($"[PlatformFeatureConfig] Platform optimizations enabled: {EnablePlatformOptimizations}");
        }
    }
}
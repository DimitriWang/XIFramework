using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 默认特性配置
    /// </summary>
    [CreateAssetMenu(fileName = "DefaultFeatureConfig", menuName = "GameFramework/Feature Configs/Default")]
    public class DefaultFeatureConfig : FeatureConfig
    {
        [Header("渲染设置")]
        public bool EnablePostProcessing = true;
        public int TargetFrameRate = 60;
        
        [Header("音频设置")]
        public bool EnableSpatialAudio = true;
        [Range(0f, 1f)]
        public float MasterVolume = 1f;
        
        [Header("输入设置")]
        public bool EnableGamepadSupport = true;
        public bool EnableTouchSupport = true;
        
        public override void ApplyConfig(IGameWorld world)
        {
            if (!IsEnabled)
                return;
                
            Debug.Log($"[DefaultFeatureConfig] Applying config to world: {world.Name}");
            
            // 应用渲染设置
            ApplyRenderingSettings();
            
            // 应用音频设置
            ApplyAudioSettings();
            
            // 应用输入设置
            ApplyInputSettings();
        }
        
        private void ApplyRenderingSettings()
        {
            Application.targetFrameRate = TargetFrameRate;
            Debug.Log($"[DefaultFeatureConfig] Target frame rate set to: {TargetFrameRate}");
        }
        
        private void ApplyAudioSettings()
        {
            // 音频设置应用逻辑
            Debug.Log($"[DefaultFeatureConfig] Audio master volume set to: {MasterVolume}");
        }
        
        private void ApplyInputSettings()
        {
            // 输入设置应用逻辑
            Debug.Log($"[DefaultFeatureConfig] Gamepad support: {EnableGamepadSupport}, Touch support: {EnableTouchSupport}");
        }
    }
}
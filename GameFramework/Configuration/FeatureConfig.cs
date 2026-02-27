using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 特性配置基类
    /// </summary>
    public abstract class FeatureConfig : ScriptableObject
    {
        [Header("基本配置")]
        public string FeatureName;
        public bool IsEnabled = true;
        
        [Header("优先级设置")]
        [Range(0, 100)]
        public int Priority = 50;
        
        /// <summary>
        /// 应用配置到游戏世界
        /// </summary>
        /// <param name="world">目标游戏世界</param>
        public abstract void ApplyConfig(IGameWorld world);
    }
}
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏特性基类
    /// </summary>
    public abstract class GameFeature : ScriptableObject
    {
        public enum FeatureScope
        {
            Global,
            World,
            Player
        }
        
        public FeatureScope Scope { get; set; } = FeatureScope.World;
        public string FeatureName { get; set; }
        public bool IsEnabled { get; set; } = true;
        
        public virtual void Initialize(IGameWorld world)
        {
            // 特性初始化逻辑
        }
        
        public virtual void Shutdown()
        {
            // 特性关闭逻辑
        }
        
        public virtual void SetParameter(string key, string value)
        {
            // 设置特性参数
        }
        public virtual void Activate()
        {
        }
        public virtual void Deactivate()
        {
        }
        public virtual void UpdateFeature(float deltaTime)
        {
        }
    }
}
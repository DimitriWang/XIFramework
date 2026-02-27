using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 调试子系统 - 提供调试功能
    /// 自动创建在GameInstance级别
    /// </summary>
    [AutoCreateSubsystem(Priority = -100)]
    public class DebugSubsystem : GameInstanceSubsystem
    {
        public bool IsDebugEnabled { get; set; } = true;
        
        public override void Initialize()
        {
            base.Initialize();
            
            if (GameInstance?.Configuration != null)
            {
                IsDebugEnabled = GameInstance.Configuration.EnableDebug;
            }
            
            Debug.Log("[DebugSubsystem] Initialized");
        }
        
        public override void Update(float deltaTime)
        {
            if (!IsDebugEnabled)
                return;
                
            // 调试信息更新
            // 例如：显示FPS、内存使用情况等
        }
        
        public override void Shutdown()
        {
            base.Shutdown();
            Debug.Log("[DebugSubsystem] Shutdown");
        }
        
        public void LogInfo(string message)
        {
            if (IsDebugEnabled)
            {
                Debug.Log($"[GameDebug] {message}");
            }
        }
        
        public void LogWarning(string message)
        {
            if (IsDebugEnabled)
            {
                Debug.LogWarning($"[GameDebug] {message}");
            }
        }
        
        public void LogError(string message)
        {
            Debug.LogError($"[GameDebug] {message}");
        }
    }
}

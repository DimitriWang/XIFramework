using XIFramework.GameLaunch;

namespace XIFramework.GameFramework.Utility
{
    /// <summary>
    /// GameInstance便捷访问助手 - 提供全局静态访问入口
    /// </summary>
    public static class GameInstanceAssistant
    {
        /// <summary>
        /// 获取当前GameInstance
        /// </summary>
        public static IGameInstance GameInstance => 
            GameEngine.Instance?.GameInstance;
    
        /// <summary>
        /// 获取活动世界上下文
        /// </summary>
        public static IWorldContext ActiveWorldContext => 
            GameEngine.Instance?.GetActiveWorldContext();
    
        /// <summary>
        /// 获取活动游戏模式
        /// </summary>
        public static IGameMode ActiveGameMode => 
            ActiveWorldContext?.GameWorld?.GameMode;
        
        /// <summary>
        /// 获取活动游戏世界
        /// </summary>
        public static IGameWorld ActiveGameWorld =>
            ActiveWorldContext?.GameWorld;
    
        /// <summary>
        /// 快速获取GameInstance级子系统
        /// </summary>
        public static T GetGlobalSubsystem<T>() where T : class, ISubsystem => 
            GameInstance?.GetSubsystem<T>();
    
        /// <summary>
        /// 快速获取World级子系统
        /// </summary>
        public static T GetWorldSubsystem<T>() where T : class, ISubsystem =>
            ActiveWorldContext?.GetSubsystem<T>();
    }
}

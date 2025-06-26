using XIFramework.GameLaunch;

namespace XIFramework.GameFramework.Utility
{
    public static class GameInstanceAssistant
    {
        public static XIGameInstance GameInstance => 
            GameEngine.Instance?.GetGameInstance();
    
        // 获取活动世界上下文
        public static XIWorldContext ActiveWorldContext => 
            GameEngine.Instance?.GetActiveWorldContext();
    
        // 获取活动游戏模式
        public static XIGameMode ActiveGameMode => 
            ActiveWorldContext?.GameWorld?.GameMode;
    
        // 快速获取子系统 (全局)
        public static T GetGlobalSubsystem<T>() where T : XIGameInstanceSubsystem => 
            GameInstance?.GetSubsystem<T>();
    
        // 快速获取子系统 (世界)
        public static T GetWorldSubsystem<T>() where T : XIWorldSubsystem => 
            ActiveWorldContext?.GetSubsystem<T>();
    }
}
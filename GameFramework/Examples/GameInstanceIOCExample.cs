using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// GameInstance IOC使用示例
    /// 展示如何使用GameInstance的IOC容器和子系统
    /// 注意：实际项目中应从GameLaunch层通过GameInstanceAssistant获取GameInstance
    /// </summary>
    public class GameInstanceIOCExample : MonoBehaviour
    {
        /// <summary>
        /// 外部注入GameInstance引用（由GameLaunch层设置）
        /// </summary>
        private IGameInstance _gameInstance;
        
        /// <summary>
        /// 设置GameInstance引用
        /// 通常由GameLaunch层的代码调用，例如：
        /// example.SetGameInstance(GameInstanceAssistant.GameInstance);
        /// </summary>
        public void SetGameInstance(IGameInstance gameInstance)
        {
            _gameInstance = gameInstance;
        }
        
        private void Start()
        {
            if (_gameInstance == null)
            {
                Debug.LogWarning("[IOCExample] GameInstance not set. Call SetGameInstance() first.");
                return;
            }
            
            // 示例：注册自定义服务到全局容器
            RegisterCustomServices(_gameInstance);
            
            // 示例：获取子系统
            UseSubsystems(_gameInstance);
        }
        
        private void RegisterCustomServices(IGameInstance gameInstance)
        {
            // 注册实例
            var customService = new CustomGameService();
            gameInstance.GlobalContainer.Register<ICustomGameService>(customService);
            
            // 注册类型映射
            gameInstance.GlobalContainer.Register<IGameLogger, ConsoleGameLogger>();
            
            Debug.Log("[IOCExample] Custom services registered");
        }
        
        private void UseSubsystems(IGameInstance gameInstance)
        {
            // 获取调试子系统
            var debugSubsystem = gameInstance.GetSubsystem<DebugSubsystem>();
            debugSubsystem?.LogInfo("GameInstance IOC example is running");
            
            // 获取全局服务
            var customService = gameInstance.GlobalContainer.Resolve<ICustomGameService>();
            customService?.DoSomething();
            
            // 获取Logger服务
            var logger = gameInstance.GlobalContainer.Resolve<IGameLogger>();
            logger?.Log("Service found and working!");
        }
    }
    
    // 示例接口和实现
    public interface ICustomGameService
    {
        void DoSomething();
    }
    
    public class CustomGameService : ICustomGameService
    {
        public void DoSomething()
        {
            Debug.Log("[CustomService] Doing something important...");
        }
    }
    
    public interface IGameLogger
    {
        void Log(string message);
    }
    
    public class ConsoleGameLogger : IGameLogger
    {
        public void Log(string message)
        {
            Debug.Log($"[GameLogger] {message}");
        }
    }
}

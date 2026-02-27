using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏实例接口 - 管理多个世界上下文
    /// 对应Unreal中的UGameInstance概念
    /// </summary>
    public interface IGameInstance
    {
        /// <summary>
        /// 游戏实例配置
        /// </summary>
        IGameInstanceConfiguration Configuration { get; }
        
        /// <summary>
        /// 当前激活的世界上下文
        /// </summary>
        IWorldContext ActiveWorldContext { get; }
        
        /// <summary>
        /// 全局IOC容器（用于高级扩展）
        /// </summary>
        IXIFrameworkContainer GlobalContainer { get; }
        
        /// <summary>
        /// 游戏实例是否已初始化
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// 世界上下文激活事件
        /// </summary>
        event Action<IWorldContext> OnWorldContextActivated;
        
        /// <summary>
        /// 世界上下文停用事件
        /// </summary>
        event Action<IWorldContext> OnWorldContextDeactivated;
        
        /// <summary>
        /// 初始化游戏实例
        /// </summary>
        /// <param name="configuration">游戏实例配置</param>
        void Initialize(IGameInstanceConfiguration configuration);
        
        /// <summary>
        /// 更新游戏实例（由外部驱动）
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 关闭游戏实例
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// 创建世界上下文
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        /// <param name="settings">世界设置（null则使用默认设置）</param>
        /// <returns>创建的世界上下文</returns>
        IWorldContext CreateWorldContext(string contextName, IWorldSettings settings = null);
        
        /// <summary>
        /// 销毁世界上下文
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        void DestroyWorldContext(string contextName);
        
        /// <summary>
        /// 设置激活的世界上下文
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        void SetActiveWorldContext(string contextName);
        
        /// <summary>
        /// 获取世界上下文
        /// </summary>
        /// <param name="contextName">上下文名称</param>
        /// <returns>世界上下文实例</returns>
        IWorldContext GetWorldContext(string contextName);
        
        /// <summary>
        /// 获取GameInstance级别的子系统
        /// </summary>
        /// <typeparam name="T">子系统类型</typeparam>
        /// <returns>子系统实例</returns>
        T GetSubsystem<T>() where T : class, ISubsystem;
    }
}
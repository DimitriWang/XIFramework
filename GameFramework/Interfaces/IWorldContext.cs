using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界上下文接口 - 管理世界的状态和生命周期
    /// 对应Unreal中的WorldContext概念
    /// </summary>
    public interface IWorldContext
    {
        /// <summary>
        /// 上下文名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 关联的游戏实例
        /// </summary>
        IGameInstance GameInstance { get; }
        
        /// <summary>
        /// 世界设置
        /// </summary>
        IWorldSettings Settings { get; }
        
        /// <summary>
        /// 游戏世界实例
        /// </summary>
        IGameWorld GameWorld { get; }
        
        /// <summary>
        /// 世界级IOC子容器（用于per-world服务隔离）
        /// </summary>
        IXIFrameworkContainer WorldContainer { get; }
        
        /// <summary>
        /// 上下文状态
        /// </summary>
        WorldContextState State { get; }
        
        /// <summary>
        /// 初始化上下文
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 激活上下文
        /// </summary>
        void Activate();
        
        /// <summary>
        /// 更新上下文
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 停用上下文
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// 关闭上下文
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 获取世界级别的子系统
        /// </summary>
        /// <typeparam name="T">子系统类型</typeparam>
        /// <returns>子系统实例</returns>
        T GetSubsystem<T>() where T : class, ISubsystem;
    }
    
    /// <summary>
    /// 世界上下文状态枚举
    /// </summary>
    public enum WorldContextState
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        Uninitialized,
        
        /// <summary>
        /// 已初始化但未激活
        /// </summary>
        Initialized,
        
        /// <summary>
        /// 激活状态
        /// </summary>
        Active,
        
        /// <summary>
        /// 已停用
        /// </summary>
        Inactive,
        
        /// <summary>
        /// 已关闭
        /// </summary>
        Shutdown
    }
}
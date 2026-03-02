using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 子系统接口 - 所有子系统的基接口
    /// 对应Unreal中的USubsystem概念
    /// </summary>
    public interface ISubsystem
    {
        /// <summary>
        /// 子系统生命周期类型
        /// </summary>
        SubsystemLifecycle Lifecycle { get; }
        
        /// <summary>
        /// 子系统是否已初始化
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// 初始化子系统
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 更新子系统
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 关闭子系统
        /// </summary>
        void Shutdown();
    }
    
    /// <summary>
    /// 子系统生命周期枚举
    /// </summary>
    public enum SubsystemLifecycle
    {
        /// <summary>
        /// 游戏实例级别生命周期
        /// </summary>
        GameInstance,
        
        /// <summary>
        /// 世界级别生命周期
        /// </summary>
        World,
        
        /// <summary>
        /// 动态生命周期
        /// </summary>
        Dynamic
    }
    
    /// <summary>
    /// 自动创建子系统特性 - 标记的子系统会在对应作用域初始化时自动创建
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCreateSubsystemAttribute : Attribute
    {
        /// <summary>
        /// 初始化优先级（数值越小越先初始化）
        /// </summary>
        public int Priority { get; set; } = 0;
    }
}
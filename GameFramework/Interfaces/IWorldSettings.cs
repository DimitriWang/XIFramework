using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界设置接口 - 定义世界的基本配置
    /// </summary>
    public interface IWorldSettings
    {
        /// <summary>
        /// 持久关卡名称
        /// </summary>
        string PersistentLevel { get; }
        
        /// <summary>
        /// 初始子关卡列表
        /// </summary>
        string[] SubLevels { get; }
        
        /// <summary>
        /// 游戏模式类型
        /// </summary>
        Type GameModeType { get; }
        
        /// <summary>
        /// 玩家控制器类型
        /// </summary>
        Type PlayerControllerType { get; }
        
        /// <summary>
        /// 玩家状态类型
        /// </summary>
        Type PlayerStateType { get; }
    }
}
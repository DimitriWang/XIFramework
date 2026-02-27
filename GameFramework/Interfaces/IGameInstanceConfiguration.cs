using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏实例配置接口 - 定义游戏实例的全局配置
    /// </summary>
    public interface IGameInstanceConfiguration
    {
        /// <summary>
        /// 最大玩家数量
        /// </summary>
        int MaxPlayers { get; }
        
        /// <summary>
        /// 默认游戏模式类型
        /// </summary>
        Type DefaultGameMode { get; }
        
        /// <summary>
        /// 默认世界设置
        /// </summary>
        IWorldSettings DefaultWorldSettings { get; }
        
        /// <summary>
        /// 是否启用调试模式
        /// </summary>
        bool EnableDebug { get; }
    }
}
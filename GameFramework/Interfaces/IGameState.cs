using System.Collections.Generic;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏状态接口 - 管理游戏全局状态
    /// 对应Unreal中的GameState概念
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// 关联的游戏世界
        /// </summary>
        IGameWorld World { get; }
        
        /// <summary>
        /// 游戏是否激活
        /// </summary>
        bool IsGameActive { get; }
        
        /// <summary>
        /// 游戏时间
        /// </summary>
        float GameTime { get; }
        
        /// <summary>
        /// 玩家状态列表
        /// </summary>
        IReadOnlyList<IPlayerState> PlayerStates { get; }
        
        /// <summary>
        /// 初始化游戏状态
        /// </summary>
        /// <param name="world">关联的世界</param>
        void Initialize(IGameWorld world);
        
        /// <summary>
        /// 启动游戏
        /// </summary>
        void StartGame();
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        void EndGame();
        
        /// <summary>
        /// 更新游戏状态
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 添加玩家状态
        /// </summary>
        /// <param name="playerState">玩家状态</param>
        void AddPlayerState(IPlayerState playerState);
        
        /// <summary>
        /// 获取玩家状态
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <returns>玩家状态</returns>
        IPlayerState GetPlayerState(int playerId);
    }
}
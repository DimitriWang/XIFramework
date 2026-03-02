using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏模式接口 - 控制游戏规则和逻辑
    /// 对应Unreal中的AGameModeBase概念
    /// </summary>
    public interface IGameMode
    {
        /// <summary>
        /// 关联的游戏世界
        /// </summary>
        IGameWorld World { get; }
        
        /// <summary>
        /// 游戏状态
        /// </summary>
        IGameState GameState { get; }
        
        /// <summary>
        /// 是否游戏已开始
        /// </summary>
        bool IsGameStarted { get; }
        
        /// <summary>
        /// 玩家控制器数组
        /// </summary>
        IPlayerController[] Players { get; }
        
        /// <summary>
        /// 子关卡加载事件
        /// </summary>
        event Action<string> OnSubLevelLoaded;
        
        /// <summary>
        /// 子关卡卸载事件
        /// </summary>
        event Action<string> OnSubLevelUnloaded;
        
        /// <summary>
        /// 初始化游戏模式
        /// </summary>
        /// <param name="world">关联的世界</param>
        void Initialize(IGameWorld world);
        
        /// <summary>
        /// 启动游戏
        /// </summary>
        void StartGame();
        
        /// <summary>
        /// 加载子关卡
        /// </summary>
        /// <param name="levelName">关卡名称</param>
        void LoadSubLevel(string levelName);
        
        /// <summary>
        /// 卸载子关卡
        /// </summary>
        /// <param name="levelName">关卡名称</param>
        void UnloadSubLevel(string levelName);
        
        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <returns>玩家控制器</returns>
        IPlayerController CreatePlayer(int playerId);
        
        /// <summary>
        /// 更新游戏逻辑
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        void EndGame();
    }
}
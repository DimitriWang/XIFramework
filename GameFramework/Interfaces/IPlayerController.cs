namespace XIFramework.GameFramework
{
    /// <summary>
    /// 玩家控制器接口 - 控制玩家行为
    /// 对应Unreal中的APlayerController概念
    /// </summary>
    public interface IPlayerController
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        int PlayerId { get; }
        
        /// <summary>
        /// 关联的游戏世界
        /// </summary>
        IGameWorld World { get; }
        
        /// <summary>
        /// 玩家状态
        /// </summary>
        IPlayerState PlayerState { get; set; }
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// 初始化玩家控制器
        /// </summary>
        /// <param name="world">游戏世界</param>
        /// <param name="playerId">玩家ID</param>
        void Initialize(IGameWorld world, int playerId);
        
        /// <summary>
        /// 更新玩家逻辑
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 销毁玩家控制器
        /// </summary>
        void Destroy();
    }
}
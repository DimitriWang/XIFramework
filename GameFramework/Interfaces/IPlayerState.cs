namespace XIFramework.GameFramework
{
    /// <summary>
    /// 玩家状态接口 - 存储玩家数据
    /// 对应Unreal中的APlayerState概念
    /// 具体游戏属性（如血量、分数）应在子类中定义
    /// </summary>
    public interface IPlayerState
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
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// 初始化玩家状态
        /// </summary>
        /// <param name="world">游戏世界</param>
        /// <param name="playerId">玩家ID</param>
        void Initialize(IGameWorld world, int playerId);
        
        /// <summary>
        /// 更新玩家状态
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 销毁玩家状态
        /// </summary>
        void Destroy();
    }
}
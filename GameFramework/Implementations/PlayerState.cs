using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 玩家状态基类实现 - 实现IPlayerState接口
    /// 具体游戏属性（如血量、分数、经验等）应在子类中定义
    /// </summary>
    public class PlayerState : IPlayerState
    {
        #region Properties
        
        public int PlayerId { get; private set; }
        public IGameWorld World { get; private set; }
        public bool IsInitialized { get; private set; }
        
        #endregion
        
        #region IPlayerState Implementation
        
        public virtual void Initialize(IGameWorld world, int playerId)
        {
            if (IsInitialized)
                return;
                
            World = world;
            PlayerId = playerId;
            IsInitialized = true;
            
            OnInitialize();
            
            Debug.Log($"[PlayerState] Player {PlayerId} state initialized in world {World.Name}");
        }
        
        public virtual void Update(float deltaTime)
        {
            if (!IsInitialized)
                return;
        }
        
        public virtual void Destroy()
        {
            if (!IsInitialized)
                return;
                
            Debug.Log($"[PlayerState] Player {PlayerId} state destroyed");
            
            OnDestroy();
            
            World = null;
            IsInitialized = false;
        }
        
        #endregion
        
        #region Protected Virtual Methods
        
        /// <summary>
        /// 初始化回调，子类可重写以初始化自定义属性
        /// </summary>
        protected virtual void OnInitialize()
        {
        }
        
        /// <summary>
        /// 销毁回调，子类可重写以清理自定义数据
        /// </summary>
        protected virtual void OnDestroy()
        {
        }
        
        #endregion
    }
}

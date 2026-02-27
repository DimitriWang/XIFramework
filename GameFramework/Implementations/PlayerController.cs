using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 玩家控制器基类 - 实现IPlayerController接口
    /// </summary>
    public class PlayerController : IPlayerController
    {
        #region Properties
        
        public int PlayerId { get; private set; }
        public IGameWorld World { get; private set; }
        public IPlayerState PlayerState { get; set; }
        public bool IsInitialized { get; private set; }
        
        #endregion
        
        #region IPlayerController Implementation
        
        public virtual void Initialize(IGameWorld world, int playerId)
        {
            if (IsInitialized)
                return;
                
            World = world;
            PlayerId = playerId;
            IsInitialized = true;
            
            OnInitialize();
            
            Debug.Log($"[PlayerController] Player {PlayerId} initialized in world {World.Name}");
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
                
            Debug.Log($"[PlayerController] Player {PlayerId} destroyed");
            
            OnDestroy();
            
            // 同时销毁关联的PlayerState
            PlayerState?.Destroy();
            
            World = null;
            PlayerState = null;
            IsInitialized = false;
        }
        
        #endregion
        
        #region Protected Virtual Methods
        
        /// <summary>
        /// 初始化回调，子类可重写
        /// </summary>
        protected virtual void OnInitialize()
        {
        }
        
        /// <summary>
        /// 销毁回调，子类可重写
        /// </summary>
        protected virtual void OnDestroy()
        {
        }
        
        #endregion
    }
}
using System;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏模式基类 - 实现IGameMode接口
    /// </summary>
    public abstract class GameMode : IGameMode
    {
        #region Properties
        
        public IGameWorld World { get; private set; }
        public IGameState GameState { get; private set; }
        public bool IsGameStarted { get; private set; }
        public IPlayerController[] Players { get; protected set; }
        
        #endregion
        
        #region Events
        
        public event Action<string> OnSubLevelLoaded;
        public event Action<string> OnSubLevelUnloaded;
        
        #endregion
        
        #region IGameMode Implementation
        
        public virtual void Initialize(IGameWorld world)
        {
            World = world ?? throw new ArgumentNullException(nameof(world));
            GameState = world.GameState ?? throw new InvalidOperationException("GameState is not initialized");
            
            Debug.Log($"[GameMode] Initialized in world {World.Name}");
        }
        
        public virtual void StartGame()
        {
            if (IsGameStarted)
                return;
                
            Debug.Log($"[GameMode] Starting game in world {World.Name}");
            
            CreateInitialPlayers();
            
            IsGameStarted = true;
        }
        
        public virtual void LoadSubLevel(string levelName)
        {
            World.LoadSubLevel(levelName);
            OnSubLevelLoaded?.Invoke(levelName);
        }
        
        public virtual void UnloadSubLevel(string levelName)
        {
            World.UnloadLevel(levelName);
            OnSubLevelUnloaded?.Invoke(levelName);
        }
        
        public virtual IPlayerController CreatePlayer(int playerId)
        {
            // 通过WorldContext子容器创建per-world的玩家控制器
            var worldContainer = World.Context.WorldContainer;
            
            // 获取配置中指定的PlayerController类型
            var pcType = World.Context.Settings?.PlayerControllerType;
            IPlayerController player;
            if (pcType != null)
            {
                player = worldContainer.Resolve(pcType) as IPlayerController;
            }
            else
            {
                player = new PlayerController();
            }
            player.Initialize(World, playerId);
            
            // 创建玩家状态
            var psType = World.Context.Settings?.PlayerStateType;
            IPlayerState playerState;
            if (psType != null)
            {
                playerState = worldContainer.Resolve(psType) as IPlayerState;
            }
            else
            {
                playerState = new PlayerState();
            }
            playerState.Initialize(World, playerId);
            
            // 关联玩家和状态
            player.PlayerState = playerState;
            GameState.AddPlayerState(playerState);
            
            return player;
        }
        
        public virtual void Update(float deltaTime)
        {
            if (!IsGameStarted)
                return;
                
            // 游戏逻辑更新
            // 子类可以重写此方法实现具体的游戏逻辑
        }
        
        public virtual void EndGame()
        {
            Debug.Log($"[GameMode] Ending game in world {World.Name}");
            
            // 销毁所有玩家
            if (Players != null)
            {
                foreach (var player in Players)
                {
                    player.Destroy();
                }
                Players = null;
            }
            
            IsGameStarted = false;
        }
        
        #endregion
        
        #region Protected Methods
        
        protected virtual void CreateInitialPlayers()
        {
            int playerCount = World.Context.GameInstance.Configuration.MaxPlayers;
            Players = new IPlayerController[playerCount];
            
            for (int i = 0; i < playerCount; i++)
            {
                Players[i] = CreatePlayer(i);
            }
            
            Debug.Log($"[GameMode] Created {playerCount} players");
        }
        
        #endregion
    }
}
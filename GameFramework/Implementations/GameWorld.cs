using System;
using System.Collections.Generic;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏世界实现类 - 管理具体的游戏逻辑世界
    /// 通过WorldContext的子容器创建per-world的GameMode和GameState
    /// </summary>
    public class GameWorld : IGameWorld
    {
        #region Properties
        
        public string Name { get; private set; }
        public Guid WorldId { get; private set; }
        public IWorld.WorldState State { get; private set; }
        public float Time { get; private set; }
        public float DeltaTime { get; private set; }
        
        public IWorldContext Context { get; private set; }
        public IGameMode GameMode { get; private set; }
        public IGameState GameState { get; private set; }
        public bool IsRunning { get; private set; }
        public string ActivePersistentLevel { get; private set; }
        public bool IsLevelsInitialized { get; private set; }
        public bool IsGameStarted { get; private set; }
        public IReadOnlyList<string> LoadedSubLevels => _loadedSubLevels.AsReadOnly();
        
        #endregion
        
        #region Private Fields
        
        private readonly List<string> _loadedSubLevels = new List<string>();
        
        #endregion
        
        #region Constructor
        
        public GameWorld(IWorldContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Name = context.Name;
            WorldId = Guid.NewGuid();
            State = IWorld.WorldState.Uninitialized;
        }
        
        #endregion
        
        #region IWorld Implementation
        
        public void Initialize()
        {
            if (State != IWorld.WorldState.Uninitialized)
                return;
                
            Debug.Log($"[GameWorld] Initializing world: {Name}");
            
            // 初始化关卡系统
            InitializeLevels();
            
            // 通过WorldContext子容器创建独立的GameState和GameMode
            CreateGameState();
            CreateGameMode();
            
            State = IWorld.WorldState.Initialized;
            Debug.Log($"[GameWorld] World initialized: {Name}");
        }
        
        public void Activate()
        {
            if (State != IWorld.WorldState.Initialized && 
                State != IWorld.WorldState.Inactive)
                return;
                
            Debug.Log($"[GameWorld] Activating world: {Name}");
            
            // 确保关卡已初始化
            if (!IsLevelsInitialized)
            {
                InitializeLevels();
            }
            
            // 启动游戏模式
            if (!IsGameStarted && GameMode != null)
            {
                GameMode.StartGame();
                IsGameStarted = true;
            }
            
            IsRunning = true;
            State = IWorld.WorldState.Active;
            Debug.Log($"[GameWorld] World activated: {Name}");
        }
        
        public void Update(float deltaTime)
        {
            if (State != IWorld.WorldState.Active)
                return;
                
            DeltaTime = deltaTime;
            Time += deltaTime;
            
            // 更新游戏模式
            GameMode?.Update(deltaTime);
            
            // 更新游戏状态
            GameState?.Update(deltaTime);
        }
        
        public void Deactivate()
        {
            if (State != IWorld.WorldState.Active)
                return;
                
            Debug.Log($"[GameWorld] Deactivating world: {Name}");
            
            IsRunning = false;
            
            // 结束游戏
            if (IsGameStarted)
            {
                GameMode?.EndGame();
                IsGameStarted = false;
            }
            
            State = IWorld.WorldState.Inactive;
            Debug.Log($"[GameWorld] World deactivated: {Name}");
        }
        
        public void Shutdown()
        {
            if (State == IWorld.WorldState.Shutdown)
                return;
                
            Debug.Log($"[GameWorld] Shutting down world: {Name}");
            
            // 停用世界
            if (State == IWorld.WorldState.Active)
            {
                Deactivate();
            }
            
            // 卸载所有关卡
            foreach (var level in _loadedSubLevels.ToArray())
            {
                UnloadLevel(level);
            }
            
            if (!string.IsNullOrEmpty(ActivePersistentLevel))
            {
                UnloadLevel(ActivePersistentLevel);
            }
            
            // 清理引用
            GameMode = null;
            GameState = null;
            Context = null;
            
            State = IWorld.WorldState.Shutdown;
            Debug.Log($"[GameWorld] World shutdown: {Name}");
        }
        
        #endregion
        
        #region IGameWorld Implementation
        
        public void LoadPersistentLevel(string levelName)
        {
            if (string.IsNullOrEmpty(levelName) || ActivePersistentLevel == levelName)
                return;
                
            // 卸载当前主关卡
            if (!string.IsNullOrEmpty(ActivePersistentLevel))
            {
                UnloadLevel(ActivePersistentLevel);
            }
            
            Debug.Log($"[GameWorld] Loading persistent level: {levelName}");
            
            // TODO: 实际的场景加载逻辑（使用YooAsset/Addressables/SceneManager）
            ActivePersistentLevel = levelName;
            _loadedSubLevels.Add(levelName);
        }
        
        public void LoadSubLevel(string levelName)
        {
            if (string.IsNullOrEmpty(levelName) || _loadedSubLevels.Contains(levelName))
                return;
                
            Debug.Log($"[GameWorld] Loading sub-level: {levelName}");
            
            // TODO: 实际的场景加载逻辑
            _loadedSubLevels.Add(levelName);
        }
        
        public void UnloadLevel(string levelName)
        {
            if (!_loadedSubLevels.Contains(levelName))
                return;
                
            Debug.Log($"[GameWorld] Unloading level: {levelName}");
            
            // TODO: 实际的场景卸载逻辑
            _loadedSubLevels.Remove(levelName);
            
            if (ActivePersistentLevel == levelName)
            {
                ActivePersistentLevel = null;
            }
        }
        
        public void InitializeLevels()
        {
            if (IsLevelsInitialized)
                return;
                
            if (Context?.Settings == null)
                return;
                
            Debug.Log($"[GameWorld] Initializing levels for world: {Name}");
            
            // 加载主关卡
            if (!string.IsNullOrEmpty(Context.Settings.PersistentLevel))
            {
                LoadPersistentLevel(Context.Settings.PersistentLevel);
            }
            
            // 加载初始子关卡
            if (Context.Settings.SubLevels != null)
            {
                foreach (var subLevel in Context.Settings.SubLevels)
                {
                    LoadSubLevel(subLevel);
                }
            }
            
            IsLevelsInitialized = true;
            Debug.Log($"[GameWorld] Levels initialized for world: {Name}");
        }
        
        public void StartGame()
        {
            if (IsGameStarted || GameMode == null)
                return;
                
            GameMode.StartGame();
            IsGameStarted = true;
        }
        
        public void EndGame()
        {
            if (!IsGameStarted)
                return;
                
            GameMode?.EndGame();
            IsGameStarted = false;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// 通过WorldContext子容器创建每个World独立的GameState
        /// </summary>
        private void CreateGameState()
        {
            if (GameState != null) return;
            
            // 从子容器创建，保证每个World都有独立的GameState实例
            var worldContainer = Context.WorldContainer;
            GameState = worldContainer.Resolve<IGameState>();
            GameState.Initialize(this);
            
            // 注册到世界容器供其他组件获取
            worldContainer.Register<IGameState>(GameState);
        }
        
        /// <summary>
        /// 通过WorldContext子容器创建每个World独立的GameMode
        /// </summary>
        private void CreateGameMode()
        {
            if (GameMode != null) return;
            
            var gameModeType = Context.Settings?.GameModeType ?? 
                              Context.GameInstance.Configuration.DefaultGameMode;
            
            if (gameModeType == null)
            {
                Debug.LogWarning($"[GameWorld] No GameMode type configured for world: {Name}");
                return;
            }
            
            // 通过子容器解析GameMode
            GameMode = Context.WorldContainer.Resolve(gameModeType) as IGameMode;
            GameMode?.Initialize(this);
        }
        
        #endregion
    }
}

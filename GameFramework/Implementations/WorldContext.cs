using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界上下文实现类 - 管理一个独立世界的完整生命周期
    /// 每个WorldContext拥有独立的IOC子容器，实现per-world服务隔离
    /// </summary>
    public class WorldContext : IWorldContext
    {
        #region Properties
        
        public string Name { get; private set; }
        public IGameInstance GameInstance { get; private set; }
        public IWorldSettings Settings { get; private set; }
        public IGameWorld GameWorld { get; private set; }
        public IXIFrameworkContainer WorldContainer { get; private set; }
        public WorldContextState State { get; private set; }
        
        #endregion
        
        #region Private Fields
        
        private XISubSystemManager _worldSubsystemManager;
        
        #endregion
        
        #region Constructor
        
        public WorldContext(string name, IGameInstance gameInstance, IWorldSettings settings)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            GameInstance = gameInstance ?? throw new ArgumentNullException(nameof(gameInstance));
            Settings = settings ?? gameInstance.Configuration.DefaultWorldSettings;
            State = WorldContextState.Uninitialized;
        }
        
        #endregion
        
        #region IWorldContext Implementation
        
        public void Initialize()
        {
            if (State != WorldContextState.Uninitialized)
                return;
                
            Debug.Log($"[WorldContext] Initializing context: {Name}");
            
            // 1. 创建世界级子容器（继承全局容器的注册）
            WorldContainer = GameInstance.GlobalContainer.CreateChildContainer();
            
            // 2. 注册 WorldContext 自身和默认per-world服务类型
            WorldContainer.Register<IWorldContext>(this);
            WorldContainer.Register<IWorldSettings>(Settings);
            WorldContainer.Register<IGameState, GameState>(isSingleton: false);
            WorldContainer.Register<IPlayerState, PlayerState>(isSingleton: false);
            WorldContainer.Register<IPlayerController, PlayerController>(isSingleton: false);
            
            // 3. 创建游戏世界
            GameWorld = new GameWorld(this);
            WorldContainer.Register<IGameWorld>(GameWorld);
            GameWorld.Initialize();
            
            // 4. 创建世界级子系统管理器并自动发现World级子系统
            _worldSubsystemManager = new XISubSystemManager(WorldContainer);
            AutoDiscoverWorldSubsystems();
            
            State = WorldContextState.Initialized;
            Debug.Log($"[WorldContext] Context initialized: {Name}");
        }
        
        public void Activate()
        {
            if (State != WorldContextState.Initialized &&
                State != WorldContextState.Inactive)
                return;
                
            Debug.Log($"[WorldContext] Activating context: {Name}");
            
            GameWorld?.Activate();
            State = WorldContextState.Active;
            
            Debug.Log($"[WorldContext] Context activated: {Name}");
        }
        
        public void Update(float deltaTime)
        {
            if (State != WorldContextState.Active)
                return;
                
            // 更新游戏世界
            GameWorld?.Update(deltaTime);
            
            // 更新世界级子系统
            _worldSubsystemManager?.UpdateSubSystems(deltaTime);
        }
        
        public void Deactivate()
        {
            if (State != WorldContextState.Active)
                return;
                
            Debug.Log($"[WorldContext] Deactivating context: {Name}");
            
            GameWorld?.Deactivate();
            State = WorldContextState.Inactive;
            
            Debug.Log($"[WorldContext] Context deactivated: {Name}");
        }
        
        public void Shutdown()
        {
            if (State == WorldContextState.Shutdown)
                return;
                
            Debug.Log($"[WorldContext] Shutting down context: {Name}");
            
            // 1. 停用
            if (State == WorldContextState.Active)
            {
                Deactivate();
            }
            
            // 2. 关闭游戏世界
            GameWorld?.Shutdown();
            GameWorld = null;
            
            // 3. 关闭世界级子系统
            _worldSubsystemManager?.ShutdownAll();
            _worldSubsystemManager = null;
            
            // 4. 清理子容器
            WorldContainer = null;
            
            State = WorldContextState.Shutdown;
            Debug.Log($"[WorldContext] Context shutdown: {Name}");
        }
        
        public T GetSubsystem<T>() where T : class, ISubsystem
        {
            return _worldSubsystemManager?.GetSubsystem<T>();
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// 自动发现并注册World级子系统
        /// 通过检查基类类型判断生命周期，避免创建临时实例
        /// </summary>
        private void AutoDiscoverWorldSubsystems()
        {
            var subsystemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => typeof(WorldSubsystem).IsAssignableFrom(t) &&
                           !t.IsInterface &&
                           !t.IsAbstract &&
                           t.GetCustomAttributes(typeof(AutoCreateSubsystemAttribute), false).Length > 0)
                .Select(t => new
                {
                    Type = t,
                    Priority = (t.GetCustomAttributes(typeof(AutoCreateSubsystemAttribute), false)
                        .FirstOrDefault() as AutoCreateSubsystemAttribute)?.Priority ?? 0
                })
                .OrderBy(x => x.Priority)
                .ToList();
            
            foreach (var entry in subsystemTypes)
            {
                try
                {
                    _worldSubsystemManager.RegisterSubSystem(entry.Type);
                    Debug.Log($"[WorldContext] Registered world subsystem: {entry.Type.Name}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[WorldContext] Failed to register subsystem {entry.Type.Name}: {ex.Message}");
                }
            }
        }
        
        #endregion
    }
}

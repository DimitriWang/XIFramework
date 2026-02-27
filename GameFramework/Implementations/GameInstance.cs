using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏实例实现类 - 纯C#类，不依赖MonoBehaviour
    /// 生命周期由外部驱动（如 GameEngine MonoBehaviour）
    /// 对应Unreal中的UGameInstance概念
    /// </summary>
    public class GameInstance : IGameInstance
    {
        #region Properties
        
        public IGameInstanceConfiguration Configuration { get; private set; }
        public IWorldContext ActiveWorldContext { get; private set; }
        public IXIFrameworkContainer GlobalContainer { get; private set; }
        public bool IsInitialized { get; private set; }
        
        #endregion
        
        #region Events
        
        public event Action<IWorldContext> OnWorldContextActivated;
        public event Action<IWorldContext> OnWorldContextDeactivated;
        
        #endregion
        
        #region Private Fields
        
        private readonly Dictionary<string, WorldContext> _worldContexts = new Dictionary<string, WorldContext>();
        private XISubSystemManager _subsystemManager;
        
        #endregion
        
        #region IGameInstance Implementation
        
        public void Initialize(IGameInstanceConfiguration configuration)
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[GameInstance] Already initialized");
                return;
            }
            
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            Debug.Log("[GameInstance] Initializing game instance...");
            
            // 1. 创建全局IOC容器
            GlobalContainer = new XIFrameworkContainer();
            
            // 2. 注册核心服务
            GlobalContainer.Register<IGameInstance>(this);
            GlobalContainer.Register<IGameInstanceConfiguration>(Configuration);
            
            // 3. 子类可在此注册额外服务
            RegisterServices();
            
            // 4. 创建子系统管理器并自动发现GameInstance级子系统
            _subsystemManager = new XISubSystemManager(GlobalContainer);
            AutoDiscoverSubsystems(SubsystemLifecycle.GameInstance);
            
            IsInitialized = true;
            Debug.Log("[GameInstance] Game instance initialized");
        }
        
        public void Update(float deltaTime)
        {
            if (!IsInitialized) return;
            
            // 更新当前激活的世界上下文
            ActiveWorldContext?.Update(deltaTime);
            
            // 更新GameInstance级子系统
            _subsystemManager?.UpdateSubSystems(deltaTime);
        }
        
        public void Shutdown()
        {
            if (!IsInitialized) return;
            
            Debug.Log("[GameInstance] Shutting down game instance...");
            
            // 1. 销毁所有世界上下文
            foreach (var contextName in _worldContexts.Keys.ToArray())
            {
                DestroyWorldContext(contextName);
            }
            
            // 2. 关闭所有GameInstance级子系统
            _subsystemManager?.ShutdownAllSync();
            _subsystemManager = null;
            
            // 3. 清理
            GlobalContainer = null;
            ActiveWorldContext = null;
            Configuration = null;
            IsInitialized = false;
            
            Debug.Log("[GameInstance] Game instance shutdown");
        }
        
        public IWorldContext CreateWorldContext(string contextName, IWorldSettings settings = null)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("GameInstance is not initialized");
            
            if (_worldContexts.ContainsKey(contextName))
            {
                Debug.LogWarning($"[GameInstance] WorldContext '{contextName}' already exists");
                return _worldContexts[contextName];
            }
            
            Debug.Log($"[GameInstance] Creating world context: {contextName}");
            
            var effectiveSettings = settings ?? Configuration.DefaultWorldSettings;
            var context = new WorldContext(contextName, this, effectiveSettings);
            context.Initialize();
            
            _worldContexts[contextName] = context;
            
            return context;
        }
        
        public void DestroyWorldContext(string contextName)
        {
            if (!_worldContexts.TryGetValue(contextName, out var context))
                return;
                
            Debug.Log($"[GameInstance] Destroying world context: {contextName}");
            
            // 如果是当前激活的，先停用
            if (ActiveWorldContext == context)
            {
                if (context.State == WorldContextState.Active)
                {
                    OnWorldContextDeactivated?.Invoke(context);
                    context.Deactivate();
                }
                ActiveWorldContext = null;
            }
            
            context.Shutdown();
            _worldContexts.Remove(contextName);
        }
        
        public void SetActiveWorldContext(string contextName)
        {
            if (!_worldContexts.TryGetValue(contextName, out var context))
            {
                Debug.LogWarning($"[GameInstance] WorldContext '{contextName}' not found");
                return;
            }
            
            if (ActiveWorldContext == context) return;
            
            Debug.Log($"[GameInstance] Setting active world context: {contextName}");
            
            // 停用当前上下文
            if (ActiveWorldContext != null && ActiveWorldContext.State == WorldContextState.Active)
            {
                OnWorldContextDeactivated?.Invoke(ActiveWorldContext);
                ActiveWorldContext.Deactivate();
            }
            
            ActiveWorldContext = context;
            
            // 激活新上下文
            if (context.State == WorldContextState.Initialized || 
                context.State == WorldContextState.Inactive)
            {
                context.Activate();
                OnWorldContextActivated?.Invoke(context);
            }
        }
        
        public IWorldContext GetWorldContext(string contextName)
        {
            return _worldContexts.TryGetValue(contextName, out var context) ? context : null;
        }
        
        public T GetSubsystem<T>() where T : class, ISubsystem
        {
            return _subsystemManager?.GetSubsystem<T>();
        }
        
        #endregion
        
        #region Protected Virtual Methods（供子类扩展）
        
        /// <summary>
        /// 子类可重写此方法注册额外的全局服务
        /// 在容器创建后、子系统初始化前调用
        /// </summary>
        protected virtual void RegisterServices()
        {
            // 子类重写以注册自定义服务
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// 自动发现并注册指定生命周期的子系统
        /// </summary>
        private void AutoDiscoverSubsystems(SubsystemLifecycle lifecycle)
        {
            var subsystemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => typeof(ISubsystem).IsAssignableFrom(t) &&
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
                    // 仅通过检查基类来确定生命周期，避免创建临时实例
                    if (lifecycle == SubsystemLifecycle.GameInstance &&
                        typeof(GameInstanceSubsystem).IsAssignableFrom(entry.Type))
                    {
                        _subsystemManager.RegisterSubSystem(entry.Type);
                        Debug.Log($"[GameInstance] Registered subsystem: {entry.Type.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameInstance] Failed to register subsystem {entry.Type.Name}: {ex.Message}");
                }
            }
        }
        
        #endregion
    }
}

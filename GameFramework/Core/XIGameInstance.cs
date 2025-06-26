using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
    public interface IConfigurable
    {
        void LoadConfiguration();
    }
    public abstract class XIGameInstance : MonoBehaviour, IConfigurable
    {
        [SerializeField] private GameInstanceConfiguration _configuration;

        private readonly Dictionary<string, XIWorldContext> _worldContexts = new();
        private XIWorldContext _activeWorldContext;
        private IXIFrameworkContainer _globalContainer;
        public GameInstanceConfiguration Configuration => _configuration;
        public XIWorldContext ActiveWorldContext => _activeWorldContext;
        public IXIFrameworkContainer GlobalContainer => _globalContainer;
        
        // 添加世界切换事件
        public event Action<XIWorldContext> OnWorldContextActivated;
        public event Action<XIWorldContext> OnWorldContextDeactivated;
        
        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeContainer();
            LoadConfiguration();
        }
        private void InitializeContainer()
        {
            _globalContainer = new XIFrameworkContainer();

            // 注册自身到容器
            _globalContainer.Register<XIGameInstance>(this);
            _globalContainer.Register<IConfigurable>(this);

            // 注册核心服务
            //_globalContainer.Register<IEventSystem>(new EventSystem());
            // _globalContainer.Register<IObjectPool>(new ObjectPool());

            // 初始化全局子系统
            InitializeGlobalSubsystems();
        }
        public void LoadConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = Resources.Load<GameInstanceConfiguration>("DefaultGameInstanceConfig");
                if (_configuration == null) _configuration = CreateDefaultConfiguration();
            }

            // 注册配置到容器
            _globalContainer.Register(_configuration);
        }
        private GameInstanceConfiguration CreateDefaultConfiguration()
        {
            var config = ScriptableObject.CreateInstance<GameInstanceConfiguration>();
            config.name = "DefaultGameInstanceConfig";
            config._defaultGameMode = typeof(DefaultGameMode);
            return config;
        }
        private void InitializeGlobalSubsystems()
        {
            // 自动创建所有全局子系统
            var subsystemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(XIGameInstanceSubsystem)) 
                         && t.IsDefined(typeof(AutoCreateSubsystemAttribute), false));
            
            foreach (var type in subsystemTypes)
            {
                var subsystem = (XIGameInstanceSubsystem)_globalContainer.Resolve(type);
                subsystem.GameInstance = this;
                subsystem.Initialize();
            }
        }

        public T GetSubsystem<T>() where T : XIGameInstanceSubsystem
        {
            return _globalContainer.Resolve<T>();
        }
        public async UniTask InitializeWorldContext(string contextName, XIWorldSettings settings = null)
        {
            await UniTask.CompletedTask; 
            if (_worldContexts.ContainsKey(contextName)) return;
            
             var context = new XIWorldContext(contextName, this, settings);
             await context.Initialize();

            _worldContexts[contextName] = context;
            if (_activeWorldContext == null)
            { 
                await SetActiveWorldContext(contextName);
            }
        }

        public async UniTask SetActiveWorldContext(string contextName)
        {
            
            if (!_worldContexts.TryGetValue(contextName, out var context)) return;
            if (_activeWorldContext == context) return;
        
            // 触发事件
            OnWorldContextDeactivated?.Invoke(_activeWorldContext);
        
            if (_activeWorldContext != null) 
            {
                await _activeWorldContext.Deactivate();
            }
        
            _activeWorldContext = context;
            
            await context.Activate();
        
            // 触发事件
            OnWorldContextActivated?.Invoke(context);
        }
        
        protected virtual void Update()
        {
            _activeWorldContext?.Update(Time.deltaTime);
            
            // 更新全局子系统
            foreach (var subsystem in _globalContainer.ResolveAll<XIGameInstanceSubsystem>())
            {
                subsystem.Update(Time.deltaTime);
            }
        }
        
        protected virtual void OnDestroy()
        {
            foreach (var context in _worldContexts.Values)
            {
                context.Shutdown().Forget();
            }
        }

        // 从容器解析所有实例
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            return _globalContainer.ResolveAll<T>();
        }
        public void SetConfiguration(GameInstanceConfiguration gameInstanceConfig)
        {
            _configuration = gameInstanceConfig;
        }
        
        
        public XIWorldContext GetWorldContext(string contextName)
        {
            return _worldContexts.TryGetValue(contextName, out var context) ? context : null;
        }
    
        public T GetWorldContext<T>(string contextName) where T : XIWorldContext
        {
            return GetWorldContext(contextName) as T;
        }
        
        public XIGameMode GetActiveGameMode()
        {
            return _activeWorldContext?.GameWorld?.GameMode;
        }
    
        public T GetActiveGameMode<T>() where T : XIGameMode
        {
            return GetActiveGameMode() as T;
        }
    }

    public class DefaultGameInstance : XIGameInstance
    {
        
    }

    //测试GameInstance类
    public class TestGameInstance : XIGameInstance
    {
        
    }
}
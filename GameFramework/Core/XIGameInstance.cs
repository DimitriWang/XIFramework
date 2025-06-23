using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
    public partial class XIGameInstance : SingletonMono<XIGameInstance>
    {
        // private static XIGameInstance _instance;
        // public static XIGameInstance Instance => _instance;
        private XIFrameworkContainer _framework;
        private XISubSystemManager _subsystemManager;
        private XIGameFeatureManager _featureManager;
        [SerializeField] protected List<XIGameFeature> _coreFeatures = new();
        // protected virtual void Awake()
        // {
        //     if (_instance != null && _instance != this)
        //     {
        //         Destroy(gameObject);
        //         return;
        //     }
        //     _instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }

        public void Update0(float deltaTime)
        {
            
        }

        public void Update1(float deltaTime)
        {
            _subsystemManager?.UpdateSubSystems(Time.deltaTime);
            _featureManager?.UpdateFeatures(Time.deltaTime);
        }

        public void Update2(float deltaTime)
        {
            
        }
        public async UniTask Initialize()
        {
            // 初始化框架容器
            _framework = new XIFrameworkContainer();

            // 注册核心系统
            _framework.Register<IXIFrameworkContainer>(_framework);
            _framework.Register(this);

            // 初始化子系统管理器
            _subsystemManager = new XISubSystemManager(_framework);
            _framework.Register(_subsystemManager);

            // 初始化GameFeature管理器
            _featureManager = new XIGameFeatureManager(_framework);
            _framework.Register(_featureManager);

            // 执行自定义初始化流程
            await CustomInitialize();
        }
        
        protected virtual async UniTask CustomInitialize()
        {
            // 默认初始化核心子系统
            InitializeCoreSubsystems();

            // 加载基础GameFeature
            LoadCoreFeatures();

            // 初始化所有子系统
            await _subsystemManager.InitializeAll();

            // 初始化所有功能模块
            await _featureManager.InitializeAll();
        }
        public async UniTask Shutdown()
        {
            await _featureManager.ShutdownAll();
            await _subsystemManager.ShutdownAll();
        }

        // 可重写以添加自定义初始化逻辑
        protected virtual void InitializeCoreSubsystems()
        {
            // 自动注册所有标记了[AutoCreateSubsystem]的子系统
            var subsystemTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsDefined(typeof(AutoCreateSubsystemAttribute), false));
            foreach (var type in subsystemTypes)
            {
                Debug.Log("AutoCreateSubsystem:"  + type.Name);
                _subsystemManager.RegisterSubSystem(type);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void LoadCoreFeatures()
        {
            //此种实现作用于直接挂载到物体上，自动创建出的GameInstance 或者 走配置的GameInstace需要额外进行实现 后续GameInstace 可作为配置类的方式实现
            foreach (var feature in _coreFeatures)
            {
                Debug.Log("LoadCoreFeature:"  + feature.GetType().Name);
                _featureManager.LoadFeature(feature);
            }
        }
        public T GetSubsystem<T>() where T : XIGameSubSystem => _subsystemManager.GetSubsystem<T>();
        public void RegisterSubSystem<T>() where T : XIGameSubSystem
        {
            _subsystemManager.RegisterSubSystem<T>();
        }
        public void RegisterSubSystem(Type subsystemType)
        {
            _subsystemManager.RegisterSubSystem(subsystemType);
        }
        public void LoadFeature(XIGameFeature feature)
        {
            _featureManager.LoadFeature(feature);
        }
        public void LoadFeature<T>() where T : XIGameFeature, new()
        {
            _featureManager.LoadFeature<T>();
        }
    }

// 扩展GameInstance类 (partial实现)
}
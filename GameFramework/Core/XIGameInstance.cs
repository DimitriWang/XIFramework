using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{


public partial class XIGameInstance : MonoBehaviour
{
    private static XIGameInstance _instance;
    public static XIGameInstance Instance => _instance;
    
    private XIFrameworkContainer _framework;
    private XISubSystemManager _subsystemManager;
    private XIGameFeatureManager _featureManager;
    
    [SerializeField] private List<XIGameFeature> _coreFeatures = new();
    
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public async UniTask Initialize()
    {
        // 初始化框架容器
        _framework = new XIFrameworkContainer();
        
        // 注册核心系统
        _framework.Register<IArchitectureContainer>(_framework);
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
        var subsystemTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsDefined(typeof(AutoCreateSubsystemAttribute), false));
        
        foreach (var type in subsystemTypes)
        {
            _subsystemManager.RegisterSubSystem(type);
        }
    }
    
    protected virtual void LoadCoreFeatures()
    {
        foreach (var feature in _coreFeatures)
        {
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
public partial class XIGameInstance
{
    // 在编辑器中添加默认核心功能模块
    #if UNITY_EDITOR
    private void Reset()
    {
        _coreFeatures = new List<XIGameFeature>
        {
            // new ResourceFeature(),
            // new UIFeature()
        };
    }
    #endif
}
}
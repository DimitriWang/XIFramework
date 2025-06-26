using Unity.Collections;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{

    using UnityEngine;
using Cysharp.Threading.Tasks;

// 游戏引擎入口点
public class GameEngine : MonoBehaviour
{
    [SerializeField] private GameInstanceConfiguration _gameInstanceConfig;


    [ReadOnly]
    [SerializeField] private XIGameInstance _activeGameInstance;
    // 全局访问点
    public static GameEngine Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private async void Start()
    {
        await StartGame();
    }
    
    public async UniTask StartGame()
    {
        // 创建GameInstance
        var go = new GameObject("GameInstance");
        var gameInstanceType =
            _gameInstanceConfig?.OverrideGameInstanceType ?? _gameInstanceConfig.DefaultGameInstanceType;
        _activeGameInstance = (XIGameInstance)go.AddComponent(gameInstanceType);
        
        // 应用配置
        if (_gameInstanceConfig != null)
        {
            _activeGameInstance.SetConfiguration(_gameInstanceConfig);
        }
        
        // 初始化主世界 - 使用配置中的默认设置
        await InitializeMainWorld();
    }
    
    private async UniTask InitializeMainWorld()
    {
        if (_activeGameInstance.Configuration.defaultWorldSettings == null)
        {
            Debug.LogError("Default world settings not configured!");
            return;
        }
        
        var mainWorldSettings = _activeGameInstance.Configuration.defaultWorldSettings;
        
        // 统一使用"MainWorldContext"名称
        await _activeGameInstance.InitializeWorldContext("MainWorldContext", mainWorldSettings);
        await _activeGameInstance.SetActiveWorldContext("MainWorldContext");

// #if UNITY_EDITOR
//         await _activeGameInstance.InitializeWorldContext("EditorPlayWorldContext", mainWorldSettings);
// #else
//         await _activeGameInstance.InitializeWorldContext("RuntimePlayWorldContext", mainWorldSettings);
// #endif

    }


    // 新增：获取当前游戏实例
    public XIGameInstance GetGameInstance() => _activeGameInstance;

    // 新增：直接获取活动世界上下文
    public XIWorldContext GetActiveWorldContext() =>
        _activeGameInstance?.ActiveWorldContext;

}

public class GameLaunch : XIGameMode
{
    public override void StartGame()
    {
        Debug.Log("Game Launch started");
    }
}

// 菜单GameMode
public class MenuGameMode : XIGameMode
{
    public override void StartGame()
    {
        base.StartGame();
        Debug.Log("Menu Game Mode started");
        
        // 打开主菜单
        OpenMainMenu();
    }
    
    private void OpenMainMenu()
    {
        // 实现菜单打开逻辑
        Debug.Log("Opening main menu...");
    }
}
}
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{

    using UnityEngine;
using Cysharp.Threading.Tasks;

// 游戏引擎入口点
public class GameEngine : MonoBehaviour
{
    [SerializeField] private GameInstanceConfiguration _gameInstanceConfig;
    [SerializeField] private System.Type _gameInstanceType = typeof(XIGameInstance);
    
    private XIGameInstance _activeGameInstance;
    
    private async void Start()
    {
        await StartGame();
    }
    
    public async UniTask StartGame()
    {
        // 创建GameInstance
        var go = new GameObject("GameInstance");
        _activeGameInstance = (XIGameInstance)go.AddComponent(_gameInstanceType);
        
        // 应用配置
        if (_gameInstanceConfig != null)
        {
            _activeGameInstance.SetConfiguration(_gameInstanceConfig);
        }
        
        // 初始化主世界
        await InitializeMainWorld();
    }
    
    private async UniTask InitializeMainWorld()
    {
        // 创建主世界上下文
        var mainWorldSettings = new XIWorldSettings()
        {
            SceneName = "GameLaunch",
            gameModeType = typeof(GameLaunch),
            isPersistent = true
        };

#if UNITY_EDITOR
        await _activeGameInstance.InitializeWorldContext("EditorPlayWorldContext", mainWorldSettings);
#else
        await _activeGameInstance.InitializeWorldContext("RuntimePlayWorldContext", mainWorldSettings);
#endif

    }
    
    public async UniTask SwitchToGameWorld(string worldName, XIWorldSettings settings)
    {
        if (_activeGameInstance == null)
        {
            Debug.LogError("GameInstance not initialized");
            return;
        }
        
        var contextName = $"GameContext_{worldName}";
        
        if (_activeGameInstance.GetWorldContext(contextName) != null)
        {
            await _activeGameInstance.InitializeWorldContext(contextName, settings);
        }
        
        await _activeGameInstance.SetActiveWorldContext(contextName);
    }
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
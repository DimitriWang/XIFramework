using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{



    public class XIGameWorld
    {
        [Inject]
        public XIWorldContext Context { get; internal set; }
        public IXIFrameworkContainer WorldContainer { get; internal set; }
        [Inject]
        public XIFeatureConfigManager FeatureConfigManager { get; internal set; }
        public XIGameMode GameMode { get; private set; }
        public XIGameState GameState { get; private set; }
        public XIGameFeatureManager FeatureManager { get; private set; }
        public bool IsRunning { get; private set; }
        public string ActivePersistentLevel { get; private set; }
        public bool IsLevelsInitialized { get; private set; }
        public bool IsGameStarted { get; private set; }
        public List<string> LoadedSubLevels { get; } = new List<string>();
        private List<string> _pendingSubLevels = new List<string>();

        
        public async UniTask Initialize()
        {
            await InitializeLevels();

            WorldContainer = Context.WorldContainer.CreateChildContainer();
            
            WorldContainer.Register(this);
        
            // 创建特性管理器
            FeatureManager = new XIGameFeatureManager(this);
            WorldContainer.Register(FeatureManager);
            
            CreateGameMode();
            
            // 加载世界特性
            await LoadWorldFeatures();
            // 加载场景
            
            // 创建世界容器
            
            // 初始化世界子系统
            
            // 初始化游戏模式
        }
        
        private async UniTask LoadWorldFeatures()
        {
            // 加载默认配置
            var defaultConfig = await FeatureConfigManager.GetConfigAsync("DefaultFeatureConfig");
            if (defaultConfig != null)
            {
                defaultConfig.ApplyConfig(this);
            }
        
            // 加载世界特定配置
            var worldConfig = await FeatureConfigManager.GetConfigAsync($"{Context.Name}Features");
            if (worldConfig != null)
            {
                worldConfig.ApplyConfig(this);
            }
        
            // 加载平台特定配置
            var platformConfig = await FeatureConfigManager.GetConfigAsync($"{Application.platform}Features");
            if (platformConfig != null)
            {
                platformConfig.ApplyConfig(this);
            }
        
            // 加载场景特定配置
            var sceneConfig = Resources.Load<WorldFeatureConfig>($"Features/{Context.Settings.PersistentLevel}");
            if (sceneConfig != null)
            {
                sceneConfig.ApplyConfig(this);
            }
        }
        
        public void CreateGameMode()
        {
            if (GameMode != null) return;
            
            var gameModeType = Context.Settings?.GameModeType ?? 
                               Context.GameInstance.Configuration.DefaultGameMode;
            
            // //✅ 确保 GameMode 类型已注册
            // if (!WorldContainer.IsRegistered(gameModeType))
            // {
            //     WorldContainer.Register(gameModeType, gameModeType);
            // }
            
            GameState = WorldContainer.Resolve<XIGameState>();
            GameState.Initialize(this);
            
            GameMode = WorldContainer.Resolve(gameModeType) as XIGameMode;
            GameMode.Initialize(this);

        }
        
        
        public async UniTask LoadPersistentLevel(string levelName)
        {
            if (ActivePersistentLevel == levelName) return;
        
            // 卸载当前主关卡
            if (!string.IsNullOrEmpty(ActivePersistentLevel))
            {
                await UnloadLevel(ActivePersistentLevel);
            }
        
            Debug.Log($"Loading persistent level: {levelName}");
            // TODO : 
            // 实际加载逻辑（使用Addressables/SceneManager）
            await UniTask.Delay(300); // 模拟加载
            ActivePersistentLevel = levelName;
        }
        
        
        public async UniTask LoadSubLevel(string levelName)
        {
            if (LoadedSubLevels.Contains(levelName)) return;
        
            Debug.Log($"Loading sub-level: {levelName}");
            // 实际加载逻辑 TODO: 
            await UniTask.Delay(200);
            LoadedSubLevels.Add(levelName);
        }
    
        public async UniTask UnloadLevel(string levelName)
        {
            Debug.Log($"Unloading level: {levelName}");
            // 实际卸载逻辑 TODO: 
            await UniTask.Delay(150);
            LoadedSubLevels.Remove(levelName);
            if (ActivePersistentLevel == levelName) ActivePersistentLevel = null;
        }
    
        // 初始化时加载关卡
        public async UniTask InitializeLevels()
        {
            if (IsLevelsInitialized) return;
            
            if (Context.Settings == null) return;
        
            // 加载主关卡
            await LoadPersistentLevel(Context.Settings.PersistentLevel);
        
            // 加载初始子关卡
            foreach (var subLevel in Context.Settings.SubLevels)
            {
                await LoadSubLevel(subLevel);
            }

            IsLevelsInitialized = true;
        }
        
        public void StartGame()
        {
            if (IsGameStarted || GameMode == null) return;
        
            GameMode.StartGame();
            IsGameStarted = true;
        }
        
        public void Update(float deltaTime)
        {
            // 更新游戏模式
            
            // 更新世界子系统
        }
        
        public void Shutdown()
        {
            // 卸载场景
            
            // 卸载世界容器
            
            // 卸载世界子系统
            
            // 卸载游戏模式
        }
        
    }
    
    

}
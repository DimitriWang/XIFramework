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
        [Inject]
        public IXIFrameworkContainer WorldContainer { get; internal set; }
        [Inject]
        public XIFeatureConfigManager FeatureConfigManager { get; internal set; }
        
        public XIGameMode GameMode { get; private set; }
        public XIGameState GameState { get; private set; }
        public XIGameFeatureManager FeatureManager { get; private set; }
    
        public bool IsRunning { get; private set; }
        
        public async UniTask Initialize()
        {
            
            
            WorldContainer.Register(this);
        
            // 创建特性管理器
            FeatureManager = new XIGameFeatureManager(this);
            WorldContainer.Register(FeatureManager);
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
            var sceneConfig = Resources.Load<WorldFeatureConfig>($"Features/{Context.Settings.SceneName}");
            if (sceneConfig != null)
            {
                sceneConfig.ApplyConfig(this);
            }
        }
        
        public void CreateGameMode()
        {
            var gameModeType = Context.Settings?.gameModeType ?? 
                               Context.GameInstance.Configuration.DefaultGameMode;
        
            GameMode = (XIGameMode)WorldContainer.Resolve(gameModeType);
            GameMode.Initialize(this);
        
            // 创建GameState
            GameState = WorldContainer.Resolve<XIGameState>();
            GameState.Initialize(this);
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
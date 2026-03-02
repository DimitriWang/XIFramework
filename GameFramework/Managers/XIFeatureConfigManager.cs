using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// Feature配置管理器 - 负责加载和缓存GameFeatureConfig
    /// </summary>
    [AutoCreateSubsystem(Priority = 10)]
    public class XIFeatureConfigManager : GameInstanceSubsystem, IAsyncInitialization
    {
        private readonly Dictionary<string, GameFeatureConfig> _configCache = new();
    
        public override void Initialize()
        {
            base.Initialize();
        }
        
        public async UniTask InitializeAsync()
        {
            await PreloadAllConfigs();
        }
    
        private async UniTask PreloadAllConfigs()
        {
            var configs = Resources.LoadAll<GameFeatureConfig>("FeatureConfigs");
        
            foreach (var config in configs)
            {
                _configCache[config.configName] = config;
                Debug.Log($"[XIFeatureConfigManager] Preloaded: {config.configName}");
            }
        
            await UniTask.Yield();
        }
    
        public GameFeatureConfig GetConfig(string configName)
        {
            if (_configCache.TryGetValue(configName, out var config))
            {
                return config;
            }
        
            Debug.LogWarning($"[XIFeatureConfigManager] Config '{configName}' not found in cache, loading directly");
            return Resources.Load<GameFeatureConfig>($"FeatureConfigs/{configName}");
        }
    
        public async UniTask<GameFeatureConfig> GetConfigAsync(string configName)
        {
            if (_configCache.TryGetValue(configName, out var config))
            {
                return config;
            }
        
            Debug.Log($"[XIFeatureConfigManager] Loading config asynchronously: {configName}");
            var asyncOp = Resources.LoadAsync<GameFeatureConfig>($"FeatureConfigs/{configName}");
            await asyncOp;
        
            if (asyncOp.asset is GameFeatureConfig loadedConfig)
            {
                _configCache[configName] = loadedConfig;
                return loadedConfig;
            }
        
            Debug.LogError($"[XIFeatureConfigManager] Failed to load config: {configName}");
            return null;
        }
        
        public override void Shutdown()
        {
            _configCache.Clear();
            base.Shutdown();
        }
    }
}
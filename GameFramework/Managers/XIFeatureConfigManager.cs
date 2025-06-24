using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public class XIFeatureConfigManager : XIGameInstanceSubsystem, IAsyncInitialization
    {
        private Dictionary<string, XIGameFeatureConfig> _configCache = new();
    
        public async UniTask InitializeAsync()
        {
            // 预加载所有配置
            await PreloadAllConfigs();
        }
    
        private async UniTask PreloadAllConfigs()
        {
            // 实际项目中可能使用Addressables
            var configs = Resources.LoadAll<XIGameFeatureConfig>("FeatureConfigs");
        
            foreach (var config in configs)
            {
                _configCache[config.configName] = config;
                Debug.Log($"Preloaded feature config: {config.configName}");
            }
        
            await UniTask.Delay(100); // 模拟加载时间
        }
    
        public XIGameFeatureConfig GetConfig(string configName)
        {
            if (_configCache.TryGetValue(configName, out var config))
            {
                return config;
            }
        
            Debug.LogWarning($"Feature config '{configName}' not found in cache, loading directly");
            return Resources.Load<XIGameFeatureConfig>($"FeatureConfigs/{configName}");
        }
    
        public async UniTask<XIGameFeatureConfig> GetConfigAsync(string configName)
        {
            if (_configCache.TryGetValue(configName, out var config))
            {
                return config;
            }
        
            // 异步加载配置
            Debug.Log($"Loading feature config asynchronously: {configName}");
            var asyncOp = Resources.LoadAsync<XIGameFeatureConfig>($"FeatureConfigs/{configName}");
            await asyncOp;
        
            if (asyncOp.asset is XIGameFeatureConfig loadedConfig)
            {
                _configCache[configName] = loadedConfig;
                return loadedConfig;
            }
        
            Debug.LogError($"Failed to load feature config: {configName}");
            return null;
        }
    }
}
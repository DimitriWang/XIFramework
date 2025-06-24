using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace XIFramework.GameFramework
{
    public class XIGameFeatureManager
    {
        [Inject] public IXIFrameworkContainer _framework { get; set; }
        private readonly Dictionary<XIGameFeature, GameFeatureConfig.FeatureLoadMode> _features = new();
        private readonly List<XIGameFeature> _activeFeatures = new();
        [Inject] private XIGameWorld World { get; set; }
        public XIGameFeatureManager() { }
        public void LoadFeature(XIGameFeature feature, GameFeatureConfig.FeatureLoadMode loadMode)
        {
            if (feature == null) return;

            // 初始化特性
            feature.Initialize();

            // 存储特性和加载模式
            _features[feature] = loadMode;

            // 根据加载模式处理
            switch (loadMode)
            {
                case GameFeatureConfig.FeatureLoadMode.Preload:
                    ActivateFeature(feature);
                    break;
                case GameFeatureConfig.FeatureLoadMode.InitializeWithWorld:
                    // 世界激活时激活
                    break;
            }
        }
        public async UniTask LoadFeatureAsync(string featureName)
        {
            // 查找按需加载的特性
            var featureEntry = _features.FirstOrDefault(f => f.Key.name == featureName && f.Value == GameFeatureConfig.FeatureLoadMode.OnDemand);
            if (featureEntry.Key != null)
            {
                await ActivateFeatureAsync(featureEntry.Key);
            }
            else
            {
                Debug.LogWarning($"Feature '{featureName}' not found or not in OnDemand mode");
            }
        }
        public void ActivateWorldFeatures()
        {
            foreach (var featureEntry in _features)
            {
                if (featureEntry.Value == GameFeatureConfig.FeatureLoadMode.InitializeWithWorld)
                {
                    ActivateFeature(featureEntry.Key);
                }
            }
        }
        private void ActivateFeature(XIGameFeature feature)
        {
            if (_activeFeatures.Contains(feature)) return;
            feature.Activate();
            _activeFeatures.Add(feature);
            Debug.Log($"Feature activated: {feature.name}");
        }
        private async UniTask ActivateFeatureAsync(XIGameFeature feature)
        {
            if (_activeFeatures.Contains(feature)) return;

            // 异步激活特性
            if (feature is IAsyncFeature asyncFeature)
            {
                await asyncFeature.ActivateAsync();
            }
            else
            {
                feature.Activate();
            }
            _activeFeatures.Add(feature);
            Debug.Log($"Feature activated asynchronously: {feature.name}");
        }
        public void DeactivateWorldFeatures()
        {
            foreach (var feature in _activeFeatures.ToList()) // ToList to avoid modification during iteration
            {
                // 只停用世界特性，全局特性保持激活
                if (feature.Scope == XIGameFeature.FeatureScope.World)
                {
                    DeactivateFeature(feature);
                }
            }
        }
        private void DeactivateFeature(XIGameFeature feature)
        {
            feature.Deactivate();
            _activeFeatures.Remove(feature);
            Debug.Log($"Feature deactivated: {feature.name}");
        }
        public void Update(float deltaTime)
        {
            foreach (var feature in _activeFeatures)
            {
                feature.UpdateFeature(deltaTime);
            }
        }
        public void Shutdown()
        {
            foreach (var feature in _activeFeatures.ToList())
            {
                DeactivateFeature(feature);
            }
            _activeFeatures.Clear();
            _features.Clear();
        }
    }
    
    public interface IAsyncFeature
    {
        UniTask ActivateAsync();
    }
}
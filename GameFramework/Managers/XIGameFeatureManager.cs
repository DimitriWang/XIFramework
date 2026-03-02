using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace XIFramework.GameFramework
{
    public class XIGameFeatureManager
    {
        public class FeatureLoadRecord
        {
            public GameFeatureConfig.FeatureLoadMode loadMode;
            public bool isLoaded;
        }
        
        private readonly Dictionary<GameFeature, FeatureLoadRecord> _features = new();
        private readonly List<GameFeature> _activeFeatures = new();
        private GameWorld World { get; set; }
        public XIGameFeatureManager(GameWorld world)
        {
            World = world;
        }
        public XIGameFeatureManager() { }
        public void LoadFeature(GameFeature feature, GameFeatureConfig.FeatureLoadMode loadMode)
        {
            if (feature == null) return;
        
            if (!_features.ContainsKey(feature))
            {
                feature.Initialize(World);
                _features[feature] = new FeatureLoadRecord {
                    loadMode = loadMode,
                    isLoaded = false
                };
            
                if (loadMode == GameFeatureConfig.FeatureLoadMode.Preload)
                {
                    ActivateFeature(feature);
                }
            }
        }
        public async UniTask LoadFeatureAsync(string featureName)
        {
            // 查找按需加载的特性
            var featureEntry = _features.FirstOrDefault(f => 
                f.Key.name == featureName && 
                f.Value.loadMode == GameFeatureConfig.FeatureLoadMode.OnDemand);
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
                if (featureEntry.Value.loadMode == GameFeatureConfig.FeatureLoadMode.InitializeWithWorld)
                {
                    ActivateFeature(featureEntry.Key);
                }
            }
        }
        private void ActivateFeature(GameFeature feature)
        {
            if (!_features.TryGetValue(feature, out var record) || record.isLoaded) 
                return;
        
            feature.Activate();
            record.isLoaded = true;
            _activeFeatures.Add(feature);
        }
        private async UniTask ActivateFeatureAsync(GameFeature feature)
        {
            if (!_features.TryGetValue(feature, out var record) || record.isLoaded) 
                return;
        
            if (feature is IAsyncFeature asyncFeature)
            {
                await asyncFeature.ActivateAsync();
            }
            else
            {
                feature.Activate();
            }
        
            record.isLoaded = true;
            _activeFeatures.Add(feature);
        }
        public void DeactivateWorldFeatures()
        {
            foreach (var feature in _activeFeatures.ToList())
            {
                if (feature.Scope == GameFeature.FeatureScope.World)
                {
                    DeactivateFeature(feature);
                }
            }
        }
        private void DeactivateFeature(GameFeature feature)
        {
            feature.Deactivate();
            _activeFeatures.Remove(feature);
        
            if (_features.TryGetValue(feature, out var record))
            {
                record.isLoaded = false;
            }
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
        
        public T GetFeature<T>() where T : GameFeature
        {
            return _features.Keys.OfType<T>().FirstOrDefault();
        }
    }
    
    public interface IAsyncFeature
    {
        UniTask ActivateAsync();
    }
}
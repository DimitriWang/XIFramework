using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public class XIGameFeatureManager
    {
        private readonly List<XIGameFeature> _features = new();
        private readonly XIFrameworkContainer _framework;
    
        public XIGameFeatureManager(XIFrameworkContainer xiFramework)
        {
            _framework = xiFramework;
        }
    
        public void LoadFeature<T>() where T : XIGameFeature, new()
        {
            var feature = new T();
            LoadFeature(feature);
        }
    
        public void LoadFeature(XIGameFeature feature)
        {
            _framework.Inject(feature);
            _features.Add(feature);
        }
    
        public async UniTask InitializeAll()
        {
            foreach (var feature in _features)
            {
                if (feature is IAsyncInitialization asyncFeature)
                {
                    await asyncFeature.InitializeAsync();
                }
                else
                {
                    feature.Initialize();
                }
            }
        }
    
        public async UniTask ShutdownAll()
        {
            foreach (var feature in _features)
            {
                if (feature is IAsyncShutdown asyncShutdown)
                {
                    await asyncShutdown.ShutdownAsync();
                }
                else
                {
                    feature.Shutdown();
                }
            }
            _features.Clear();
        }
    
        public void UpdateFeatures()
        {
            foreach (var feature in _features)
            {
                feature.Update();
            }
        }
    }
}
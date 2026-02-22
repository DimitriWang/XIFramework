using UnityEngine;
using System.Collections.Generic;

namespace XIFramework.GameFramework
{


// GameFeature 配置基类
    public class XIGameFeatureConfig : ConfigBase
    {
        public string configName = "DefaultFeatureConfig";
        public List<GameFeatureEntry> featureEntries = new();
        
        [System.Serializable]
        public class GameFeatureEntry
        {
            public XIGameFeature feature;
            public bool enabled = true;
            public FeatureLoadMode loadMode = FeatureLoadMode.InitializeWithWorld;
            [Tooltip("Additional parameters for feature initialization")]
            public List<FeatureParameter> parameters = new();
        }
        [System.Serializable]
        public class FeatureParameter
        {
            public string key;
            public string value;
        }
        public enum FeatureLoadMode
        {
            InitializeWithWorld, // 世界初始化时加载
            OnDemand, // 按需加载
            Preload // 预加载
        }
        public virtual void ApplyConfig(XIGameWorld world)
        {
            foreach (var entry in featureEntries)
            {
                if (!entry.enabled) continue;
                var feature = Instantiate(entry.feature); // 创建实例副本
                feature.Scope = XIGameFeature.FeatureScope.World;

                // 应用参数
                foreach (var param in entry.parameters)
                {
                    feature.SetParameter(param.key, param.value);
                }
                world.FeatureManager.LoadFeature(feature, entry.loadMode);
            }
        }
    }

// 默认 GameFeature 配置
    


}
using UnityEngine;

namespace XIFramework.GameFramework
{
using UnityEngine;
using System.Collections.Generic;

// GameFeature 配置基类
public abstract class GameFeatureConfig : ScriptableObject
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
        InitializeWithWorld,  // 世界初始化时加载
        OnDemand,            // 按需加载
        Preload               // 预加载
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
[CreateAssetMenu(fileName = "DefaultFeatureConfig", menuName = "Game/Feature Configs/Default")]
public class DefaultFeatureConfig : GameFeatureConfig
{
    [Header("Core Features")]
    public XIGameFeature uiFeature;
    public XIGameFeature inputFeature;
    
    [Header("Optional Features")]
    public XIGameFeature audioFeature;
    public XIGameFeature analyticsFeature;
    
    private void OnValidate()
    {
        // 确保核心特性始终启用
        featureEntries.RemoveAll(e => e.feature == uiFeature || e.feature == inputFeature);
        
        featureEntries.Add(new GameFeatureEntry {
            feature = uiFeature,
            enabled = true,
            loadMode = FeatureLoadMode.InitializeWithWorld
        });
        
        featureEntries.Add(new GameFeatureEntry {
            feature = inputFeature,
            enabled = true,
            loadMode = FeatureLoadMode.InitializeWithWorld
        });
        
        if (audioFeature != null)
        {
            featureEntries.Add(new GameFeatureEntry {
                feature = audioFeature,
                enabled = true,
                loadMode = FeatureLoadMode.Preload
            });
        }
        
        if (analyticsFeature != null)
        {
            featureEntries.Add(new GameFeatureEntry {
                feature = analyticsFeature,
                enabled = true,
                loadMode = FeatureLoadMode.OnDemand
            });
        }
    }
}

// 世界特定配置
[CreateAssetMenu(fileName = "WorldFeatureConfig", menuName = "Game/Feature Configs/World Specific")]
public class WorldFeatureConfig : GameFeatureConfig
{
    public string targetWorldName;
    
    [Header("World Specific Features")]
    public XIGameFeature worldPhysicsFeature;
    public XIGameFeature aiSystemFeature;
    
    public override void ApplyConfig(XIGameWorld world)
    {
        // 只应用于目标世界
        if (world.Context.Name != targetWorldName) return;
        
        base.ApplyConfig(world);
        
        if (worldPhysicsFeature != null)
        {
            var feature = Instantiate(worldPhysicsFeature);
            world.FeatureManager.LoadFeature(feature, FeatureLoadMode.InitializeWithWorld);
        }
        
        if (aiSystemFeature != null)
        {
            var feature = Instantiate(aiSystemFeature);
            world.FeatureManager.LoadFeature(feature, FeatureLoadMode.InitializeWithWorld);
        }
    }
}
}
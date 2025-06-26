using UnityEngine;

namespace XIFramework.GameFramework
{
    [CreateAssetMenu(fileName = "DefaultFeatureConfig", menuName = "Game/Feature Configs/Default")]
    public class DefaultFeatureConfig : XIGameFeatureConfig
    {
        [Header("Core Features")] public XIGameFeature uiFeature;
        public XIGameFeature inputFeature;
        [Header("Optional Features")] public XIGameFeature audioFeature;
        public XIGameFeature analyticsFeature;
        private void OnValidate()
        {
            // 确保核心特性始终启用
            featureEntries.RemoveAll(e => e.feature == uiFeature || e.feature == inputFeature);
            // featureEntries.Add(new GameFeatureEntry
            // {
            //     feature = uiFeature,
            //     enabled = true,
            //     loadMode = FeatureLoadMode.InitializeWithWorld
            // });
            // featureEntries.Add(new GameFeatureEntry
            // {
            //     feature = inputFeature,
            //     enabled = true,
            //     loadMode = FeatureLoadMode.InitializeWithWorld
            // });
            if (audioFeature != null)
            {
                featureEntries.Add(new GameFeatureEntry
                {
                    feature = audioFeature,
                    enabled = true,
                    loadMode = FeatureLoadMode.Preload
                });
            }
            if (analyticsFeature != null)
            {
                featureEntries.Add(new GameFeatureEntry
                {
                    feature = analyticsFeature,
                    enabled = true,
                    loadMode = FeatureLoadMode.OnDemand
                });
            }
        }
    }
}
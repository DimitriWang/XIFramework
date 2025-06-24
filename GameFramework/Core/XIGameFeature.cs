using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{

    public abstract class XIGameFeature : ScriptableObject, IAsyncInitialization
    {
        
        public enum FeatureScope
        {
            Global,     // 全局特性，附加到GameInstance
            World,      // 世界特性，附加到GameWorld
            Subsystem   // 子系统特性
        }
    
        public FeatureScope Scope = FeatureScope.Global;
        
        private Dictionary<string, string> _parameters = new();
    
        public void SetParameter(string key, string value)
        {
            _parameters[key] = value;
        }
    
        public string GetParameter(string key, string defaultValue = "")
        {
            return _parameters.TryGetValue(key, out var value) ? value : defaultValue;
        }
    
        public int GetIntParameter(string key, int defaultValue = 0)
        {
            return int.TryParse(GetParameter(key), out int result) ? result : defaultValue;
        }
    
        public float GetFloatParameter(string key, float defaultValue = 0f)
        {
            return float.TryParse(GetParameter(key), out float result) ? result : defaultValue;
        }
    
        public bool GetBoolParameter(string key, bool defaultValue = false)
        {
            return bool.TryParse(GetParameter(key), out bool result) ? result : defaultValue;
        }
        
        
        public virtual void Initialize() { }
        public virtual void Activate() { }
        public virtual void UpdateFeature(float deltaTime) { }
        public virtual void Deactivate() { }

        public virtual void Shutdown() { }

        protected T GetSubsystem<T>() where T : XIGameSubSystem => null;
          //  XIGameInstance.Instance.GetSubsystem<T>();
    
        protected void RegisterSubsystem<T>() where T : XIGameSubSystem
        {
            Debug.Log("XIGameFeature.RegisterSubsystem:" + typeof(T).Name);
            //XIGameInstance.Instance.RegisterSubSystem(typeof(T));
        }
        public async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }
    }
}
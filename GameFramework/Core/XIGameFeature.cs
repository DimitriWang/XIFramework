using UnityEngine;

namespace XIFramework.GameFramework
{

    public abstract class XIGameFeature : ScriptableObject
    {
        
        public virtual void Initialize() { }
        public virtual void Update() { }
        public virtual void Shutdown() { }
    
        protected T GetSubsystem<T>() where T : XIGameSubSystem => 
            XIGameInstance.Instance.GetSubsystem<T>();
    
        protected void RegisterSubsystem<T>() where T : XIGameSubSystem
        {
            XIGameInstance.Instance.RegisterSubSystem(typeof(T));
        }
    }
}
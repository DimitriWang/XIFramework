using UnityEngine;

namespace XIFramework.GameFramework
{

    public abstract class XIGameFeature : ScriptableObject
    {
        
        public virtual void Initialize() { }
        public virtual void UpdateFeature(float deltaTime) { }
        public virtual void Shutdown() { }

        protected T GetSubsystem<T>() where T : XIGameSubSystem => null;
          //  XIGameInstance.Instance.GetSubsystem<T>();
    
        protected void RegisterSubsystem<T>() where T : XIGameSubSystem
        {
            Debug.Log("XIGameFeature.RegisterSubsystem:" + typeof(T).Name);
            //XIGameInstance.Instance.RegisterSubSystem(typeof(T));
        }
    }
}
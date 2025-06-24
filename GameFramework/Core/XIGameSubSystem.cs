using System;

namespace XIFramework.GameFramework
{

    public abstract class XIGameSubSystem
    {
        public enum LifecycleType
        {
            GameInstance,
            World,
            Dynamic
        }

        public virtual LifecycleType Lifecycle => LifecycleType.World;
        
        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Shutdown() { }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCreateSubsystemAttribute : Attribute { }
}
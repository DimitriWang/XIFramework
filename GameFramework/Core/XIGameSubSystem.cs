using System;

namespace XIFramework.GameFramework
{

    public abstract class XIGameSubSystem
    {
        public enum LifecycleType
        {
            GameInstance,
            Scene,
            Dynamic
        }
    
        public virtual LifecycleType Lifecycle => LifecycleType.Scene;
    
        [Inject]
        protected IXIFrameworkContainer Framework { get; set; }
    
        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Shutdown() { }
    
        protected T GetSubsystem<T>() where T : XIGameSubSystem => Framework.Resolve<XISubSystemManager>().GetSubsystem<T>();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCreateSubsystemAttribute : Attribute { }
}
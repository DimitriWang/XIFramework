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
    
        [Inject]
        protected IXIFrameworkContainer Framework { get; set; }
    
        public virtual void Initialize() { }
        public virtual void Update(float deltaTime) { }
        public virtual void Shutdown() { }
    
        protected T GetSubsystem<T>() where T : XIGameSubSystem => Framework.Resolve<XISubSystemManager>().GetSubsystem<T>();
    }
    
    public abstract class XIGameInstanceSubSystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle => LifecycleType.GameInstance;
    }
    
    public abstract class XIWorldSubSystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle => LifecycleType.World;
    }
    
    public abstract class XIDynamicSubSystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle => LifecycleType.Dynamic;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCreateSubsystemAttribute : Attribute { }
}
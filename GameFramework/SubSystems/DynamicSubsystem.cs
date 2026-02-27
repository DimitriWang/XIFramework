namespace XIFramework.GameFramework
{
    /// <summary>
    /// 动态子系统基类
    /// 生命周期可以动态控制，手动创建和销毁
    /// </summary>
    public abstract class DynamicSubsystem : ISubsystem
    {
        public SubsystemLifecycle Lifecycle => SubsystemLifecycle.Dynamic;
        public bool IsInitialized { get; private set; }
        
        public virtual void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
        }
        
        public virtual void Update(float deltaTime)
        {
        }
        
        public virtual void Shutdown()
        {
            IsInitialized = false;
        }
    }
}

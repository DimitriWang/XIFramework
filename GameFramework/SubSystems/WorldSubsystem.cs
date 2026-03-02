namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界子系统基类
    /// 生命周期与WorldContext相同，在WorldContext初始化时自动创建
    /// </summary>
    public abstract class WorldSubsystem : ISubsystem
    {
        public SubsystemLifecycle Lifecycle => SubsystemLifecycle.World;
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// 关联的WorldContext（通过XISubSystemManager自动注入）
        /// </summary>
        [Inject]
        public IWorldContext WorldContext { get; private set; }
        
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

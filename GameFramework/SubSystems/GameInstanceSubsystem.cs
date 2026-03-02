namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏实例子系统基类
    /// 生命周期与GameInstance相同，在GameInstance初始化时自动创建
    /// </summary>
    public abstract class GameInstanceSubsystem : ISubsystem
    {
        public SubsystemLifecycle Lifecycle => SubsystemLifecycle.GameInstance;
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// 关联的GameInstance（通过XISubSystemManager自动注入）
        /// </summary>
        [Inject]
        public IGameInstance GameInstance { get; private set; }
        
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

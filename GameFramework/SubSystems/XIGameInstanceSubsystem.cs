namespace XIFramework.GameFramework
{
    public abstract class XIGameInstanceSubsystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle { get; } = LifecycleType.GameInstance;
        
        public virtual XIGameInstance GameInstance { get; internal set; }
    }
}
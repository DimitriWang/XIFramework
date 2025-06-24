namespace XIFramework.GameFramework
{
    public abstract class GameInstanceSubsystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle { get; } = LifecycleType.GameInstance;
        
        public virtual XIGameInstance GameInstance { get; internal set; }
    }
}
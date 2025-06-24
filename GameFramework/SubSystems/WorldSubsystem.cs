namespace XIFramework.GameFramework
{
    public abstract class XIWorldSubsystem : XIGameSubSystem
    {
        public override LifecycleType Lifecycle { get { return LifecycleType.World; } }
        
        public XIWorldContext WorldContext { get; internal set; }
        
        public XIGameWorld GameWorld { get; internal set; }
    }
}
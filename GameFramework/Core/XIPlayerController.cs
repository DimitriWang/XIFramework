namespace XIFramework.GameFramework
{
    public abstract class XIPlayerController
    {
        public XIGameWorld World { get; private set; }
        public int PlayerId { get; private set; }
        public XIPlayerState PlayerState { get; set; }
        public bool IsLocalPlayer { get; protected set; }
    
        public virtual void Initialize(XIGameWorld world, int playerId)
        {
            World = world;
            PlayerId = playerId;
            IsLocalPlayer = true; // 默认假设为本地玩家
        }
    
        public virtual void UpdateController(float deltaTime)
        {
            // 玩家输入处理
        }
    
        public virtual void Destroy()
        {
            // 清理资源
        }
    }






}
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

// 本地玩家控制器
    public class LocalPlayerController : XIPlayerController
    {
        public override void UpdateController(float deltaTime)
        {
            base.UpdateController(deltaTime);
            // 处理本地输入
        }
    }

// 网络玩家控制器
    public class NetworkPlayerController : XIPlayerController
    {
        public override void Initialize(XIGameWorld world, int playerId)
        {
            base.Initialize(world, playerId);
            IsLocalPlayer = false;
        }
        //
        // public void ProcessNetworkInput(PlayerInputData inputData)
        // {
        //     // 处理网络输入
        // }
    }
}
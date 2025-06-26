using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
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
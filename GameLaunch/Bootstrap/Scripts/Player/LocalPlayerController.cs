using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    // 本地玩家控制器
    public class LocalPlayerController : XIPlayerController
    {
        public override void UpdateController(float deltaTime)
        {
            base.UpdateController(deltaTime);
            // 处理本地输入
        }
    }
}
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{

    // 默认 单机游戏GameMode
    public class XISingleplayerGameMode : XIGameMode
    {
        public override void Initialize(XIGameWorld world)
        {
            base.Initialize(world);
            DefaultPlayerControllerType = typeof(LocalPlayerController);
        }
    
        protected override void CreateInitialPlayers()
        {
            // 只创建一个本地玩家
            Players[0] = CreatePlayer(0);
        }
    }
}
using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 单机游戏GameMode - 只创建一个本地玩家
    /// </summary>
    public class XISingleplayerGameMode : GameMode
    {
        protected override void CreateInitialPlayers()
        {
            // 单机模式只创建一个本地玩家
            Players = new IPlayerController[1];
            Players[0] = CreatePlayer(0);
            
            Debug.Log("[XISingleplayerGameMode] Created local player");
        }
    }
}

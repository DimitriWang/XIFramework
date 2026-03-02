using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 多人游戏GameMode
    /// </summary>
    public class XIMultiplayerGameMode : GameMode
    {
        public override void StartGame()
        {
            base.StartGame();
            Debug.Log("[XIMultiplayerGameMode] Multiplayer game started");
        }
    }
}

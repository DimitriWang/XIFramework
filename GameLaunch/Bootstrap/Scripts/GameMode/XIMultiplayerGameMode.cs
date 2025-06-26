using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
// 默认 多人游戏GameMode
    public class XIMultiplayerGameMode : XIGameMode
    {
        public override void Initialize(XIGameWorld world)
        {
            base.Initialize(world);
            DefaultPlayerControllerType = typeof(NetworkPlayerController);
        }
    
        public override void StartGame()
        {
            base.StartGame();
            Debug.Log("Multiplayer Game Mode started");
        
            // 网络游戏特定初始化
            InitializeNetworking();
        }
    
        private void InitializeNetworking()
        {
            // 网络初始化逻辑
        }
    }
}
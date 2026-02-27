using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 网络玩家控制器
    /// </summary>
    public class NetworkPlayerController : PlayerController
    {
        public bool IsLocalPlayer { get; private set; } = false;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log($"[NetworkPlayerController] Network player {PlayerId} ready");
        }
        
        /// <summary>
        /// 处理网络输入数据
        /// </summary>
        public virtual void ProcessNetworkInput(byte[] inputData)
        {
            // 处理网络输入
        }
    }
}

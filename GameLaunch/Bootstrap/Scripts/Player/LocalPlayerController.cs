using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 本地玩家控制器
    /// </summary>
    public class LocalPlayerController : PlayerController
    {
        public bool IsLocalPlayer { get; private set; } = true;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            // 处理本地输入初始化
            Debug.Log($"[LocalPlayerController] Local player {PlayerId} ready");
        }
    }
}

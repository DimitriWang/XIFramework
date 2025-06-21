using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    
    public class GameBootstrap : MonoBehaviour
    {
        private void Start()
        {
            AsyncStart().Forget();
        }

        public async UniTask AsyncStart()
        {
            
            // 创建GameInstance
            var gameInstance = XIGameInstance.Instance;

            // 添加自定义初始化系统
            //XIGameFramework.AddPreInitSystem<DatabasePreInitSystem>();
            // XIGameFramework.AddPostInitSystem<AnalyticsPostInitSystem>();
        
            // 初始化框架
            await XIGameFramework.Initialize(gameInstance);
            
            // 框架初始化完成，开始游戏逻辑
            
        }
    }
}
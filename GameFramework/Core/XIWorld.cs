using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{



    public class XIGameWorld
    {
        [Inject]
        public XIWorldContext Context { get; internal set; }
        
        [Inject]
        public IXIFrameworkContainer WorldContainer { get; internal set; }
        public XIGameMode GameMode { get; private set; }

        public XIGameState GameState { get; private set; }
    
        public bool IsRunning { get; private set; }
        
        
        public async UniTask Initialize()
        {
            // 加载场景
            
            // 创建世界容器
            
            // 初始化世界子系统
            
            // 初始化游戏模式
        }
        
        public void Update(float deltaTime)
        {
            // 更新游戏模式
            
            // 更新世界子系统
        }
        
        public void Shutdown()
        {
            // 卸载场景
            
            // 卸载世界容器
            
            // 卸载世界子系统
            
            // 卸载游戏模式
        }
        
    }
    
    

}
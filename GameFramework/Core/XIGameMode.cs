﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public abstract class XIGameMode
    {
        [Inject]
        public XIGameWorld World { get; internal set; }
        [Inject]
        public XIGameState GameState { get; internal set; }
        [Inject]
        public IXIFrameworkContainer WorldContainer { get; internal set; }
        
        public bool IsGameStarted { get; private set; }
        
        public XIBasePlayerController[] Players { get; protected set; }
        protected Type DefaultPlayerControllerType { get; set; } = typeof(XIBasePlayerController);
        protected Type DefaultPlayerStateType { get; set; } = typeof(XIPlayerState);
        
        
        public event Action<string> OnSubLevelLoaded;
        public event Action<string> OnSubLevelUnloaded;


        public virtual void Initialize(XIGameWorld world)
        {
            // WorldContainer = World.WorldContainer.CreateChildContainer();
            
            // DefaultPlayerControllerType = world.Context.Settings.
            // DefaultPlayerStateType = typeof(XIPlayerState);
        }
        public virtual void StartGame()
        {
            if (IsGameStarted) return;
            
            Debug.Log($"GameMode '{GetType().Name}' started in world '{World.Context.Name}'");

            CreateInitialPlayers();
            
            IsGameStarted = true;
        }
        
        public virtual async UniTask LoadSubLevel(string levelName)
        {
            await World.LoadSubLevel(levelName);
            OnSubLevelLoaded?.Invoke(levelName);
        }
    
        public virtual async UniTask UnloadSubLevel(string levelName)
        {
            await World.UnloadLevel(levelName);
            OnSubLevelUnloaded?.Invoke(levelName);
        }

        
        protected virtual void CreateInitialPlayers()
        {
            int playerCount = World.Context.GameInstance.Configuration.maxPlayers;
            Players = new XIBasePlayerController[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                Players[i] = CreatePlayer(i);
            }
        }
        
        public virtual XIBasePlayerController CreatePlayer(int playerId)
        {
            // ✅ 正确：通过容器解析玩家控制器
            var player = WorldContainer.Resolve(DefaultPlayerControllerType) as XIBasePlayerController;
            player.Initialize(World, playerId);
    
            // ✅ 正确：通过容器解析玩家状态
            var playerState = WorldContainer.Resolve(DefaultPlayerStateType) as XIPlayerState;
            playerState.Initialize(World, playerId);
        
            player.PlayerState = playerState;
            GameState.AddPlayerState(playerState);
            return player;
        }
        
        public virtual void Update(float deltaTime)
        {
            // 游戏逻辑更新
        }
        
        public virtual async UniTask EndGame()
        {
            Debug.Log($"GameMode '{GetType().Name}' ending in world '{World.Context.Name}'");

            // 销毁所有玩家
            if (Players != null)
            {
                foreach (var player in Players)
                {
                    player.Destroy();
                }
                Players = null;
            }
            await UniTask.Delay(100); // 模拟清理过程
        }



    }

    // 默认GameMode实现


    

}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public abstract class XIGameMode
    {
        [Inject]
        protected XIGameWorld World { get; private set; }
        [Inject]
        protected XIGameState GameState { get; private set; }
        [Inject]
        protected IXIFrameworkContainer WorldContainer { get; private set; }
        
        public XIPlayerController[] Players { get; protected set; }
        protected Type DefaultPlayerControllerType { get; set; } = typeof(XIPlayerController);
        protected Type DefaultPlayerStateType { get; set; } = typeof(XIPlayerState);
        public virtual void Initialize(XIGameWorld world)
        {
            
            DefaultPlayerControllerType = typeof(XIPlayerController);
            DefaultPlayerStateType = typeof(XIPlayerState);
        }
        public virtual void StartGame()
        {
            //Debug.Log($"GameMode '{GetType().Name}' started in world '{World.Context.Name}'");

            // 创建初始玩家
            CreateInitialPlayers();
        }
        
        protected virtual void CreateInitialPlayers()
        {
            int playerCount = World.Context.GameInstance.Configuration.maxPlayers;
            Players = new XIPlayerController[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                Players[i] = CreatePlayer(i);
            }
        }
        
        public virtual XIPlayerController CreatePlayer(int playerId)
        {
            var playerType = DefaultPlayerControllerType;
            var player = (XIPlayerController)Activator.CreateInstance(playerType);
            player.Initialize(World, playerId);

            // 创建PlayerState
            var playerState = (XIPlayerState)Activator.CreateInstance(DefaultPlayerStateType);
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
    public class DefaultGameMode : XIGameMode
    {
        public override void StartGame()
        {
            base.StartGame();
            Debug.Log("Default Game Mode started");
        }
    }

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
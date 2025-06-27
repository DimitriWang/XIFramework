using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public class XIWorldContext
    {
        public enum WorldState
        {
            Uninitialized,
            Initialized,
            Active,
            Inactive
        }
        
        public string Name { get; }
        public XIGameInstance GameInstance { get; }
        public XIWorldSettings Settings { get; }
        public XIGameWorld GameWorld { get; private set; }
        public IXIFrameworkContainer WorldContainer { get; private set; }
        
        private readonly List<XIWorldSubsystem> _worldSubsystems = new List<XIWorldSubsystem>();
        
        public WorldState State { get; private set; } = WorldState.Uninitialized;
        
        public XIWorldContext(string name, XIGameInstance gameInstance, XIWorldSettings settings)
        {
            Name = name;
            GameInstance = gameInstance;
            Settings = settings ?? gameInstance.Configuration.defaultWorldSettings;
        }
        
        public async UniTask Initialize()
        { 
            if (State != WorldState.Uninitialized) return;
            
            // 创建世界容器（子容器）
            WorldContainer = GameInstance.GlobalContainer.CreateChildContainer();
        
            WorldContainer.Register<IXIFrameworkContainer>(WorldContainer);
            // 注册核心服务
            WorldContainer.Register(this);
            WorldContainer.Register<XIWorldSettings>(Settings);
        
            // 创建GameWorld
            GameWorld = WorldContainer.Resolve<XIGameWorld>();
            await GameWorld.Initialize();
        
            // 初始化世界子系统
            InitializeWorldSubsystems();
            
            State = WorldState.Initialized;
        }
        
        
        private void InitializeWorldSubsystems()
        {
            // 自动创建所有世界子系统
            var subsystemTypes = typeof(XIWorldSubsystem).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(XIWorldSubsystem))
                 && t.IsDefined(typeof(AutoCreateSubsystemAttribute), false));
        
            foreach (var type in subsystemTypes)
            {
                var subsystem = (XIWorldSubsystem)WorldContainer.Resolve(type);
                subsystem.Initialize();
                _worldSubsystems.Add(subsystem);
            }
        }
        

    
        public T GetSubsystem<T>() where T : XIWorldSubsystem
        {
            return _worldSubsystems.OfType<T>().FirstOrDefault();
        }
        // 添加场景加载状态


        
        public void Update(float deltaTime)
        {
            GameWorld?.Update(deltaTime);
        
            // 更新世界子系统
            foreach (var subsystem in WorldContainer.ResolveAll<XIWorldSubsystem>())
            {
                subsystem.Update(deltaTime);
            }
        }
        
        public async UniTask Shutdown()
        {
            await Deactivate();
            
            if (!string.IsNullOrEmpty(GameWorld.ActivePersistentLevel))
            {
                await GameWorld.UnloadLevel(GameWorld.ActivePersistentLevel);
            }

            GameWorld?.Shutdown();
            GameWorld = null;
            
            State = WorldState.Uninitialized;
        }


        #region Activate Deactivate World

        public async UniTask Activate()
        {
            // 确保只从未激活状态激活
            if (State != WorldState.Initialized && State != WorldState.Inactive)
            {
                Debug.LogWarning($"Cannot activate world '{Name}' from state {State}");
                return;
            }
            
            Debug.Log($"[World] {Name} - Activating from state: {State}");

            // 确保关卡系统已初始化
            if (!GameWorld.IsLevelsInitialized)
            {
                await GameWorld.InitializeLevels();
            }

            if (GameWorld.GameMode == null)
            {
                GameWorld.CreateGameMode();
            }
            
            if (!GameWorld.GameMode.IsGameStarted)
            {
                GameWorld.GameMode.StartGame();
            }
            
            State = WorldState.Active;
        }
    
        public async UniTask Deactivate()
        {
            // 确保只从激活状态停用
            if (State != WorldState.Active)
            {
                Debug.LogWarning($"Cannot deactivate world '{Name}' from state {State}");
                return;
            }
            
            Debug.Log($"[World] {Name} - Deactivating from state: {State}");
            
            // 结束游戏模式
            if (GameWorld.GameMode != null)
            {
                await GameWorld.GameMode.EndGame();
            }
        
            if (GameWorld != null)
            {
                foreach (var level in GameWorld.LoadedSubLevels.ToArray())
                {
                    await GameWorld.UnloadLevel(level);
                }
            }
    
            State = WorldState.Inactive;
        }

        #endregion
        
        
        
        
        
        #region Unity Scene Management

        public bool IsSceneLoaded { get; private set; }
        private async UniTask LoadScene(string sceneName)
        {
            if (IsSceneLoaded) return;
        
            // 使用Unity场景管理器实际加载场景
            var loadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                sceneName, 
                UnityEngine.SceneManagement.LoadSceneMode.Single
            );
        
            await loadOp;
            IsSceneLoaded = true;
        }
    
        private async UniTask UnloadScene()
        {
            if (!IsSceneLoaded) return;
        
            // 获取当前场景
            var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        
            // 卸载场景
            var unloadOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
            await unloadOp;
        
            IsSceneLoaded = false;
        }

        #endregion
        
    }
}
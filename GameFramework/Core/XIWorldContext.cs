using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    public class XIWorldContext
    {
        public string Name { get; }
        public XIGameInstance GameInstance { get; }
        public XIWorldSettings Settings { get; }
        public XIGameWorld GameWorld { get; private set; }
        public IXIFrameworkContainer WorldContainer { get; private set; }
        
        private readonly List<XIWorldSubsystem> _worldSubsystems = new List<XIWorldSubsystem>();
        
        public XIWorldContext(string name, XIGameInstance gameInstance, XIWorldSettings settings)
        {
            Name = name;
            GameInstance = gameInstance;
            Settings = settings ?? gameInstance.Configuration.defaultWorldSettings;
        }
        
        public async UniTask Initialize()
        { 
            // 创建世界容器（子容器）
            WorldContainer = GameInstance.GlobalContainer.CreateChildContainer();
        
            // 注册核心服务
            WorldContainer.Register(this);
            WorldContainer.Register<XIWorldSettings>(Settings);
        
            // 创建GameWorld
            GameWorld = WorldContainer.Resolve<XIGameWorld>();
            GameWorld.Context = this;
            await GameWorld.Initialize();
        
            // 初始化世界子系统
            InitializeWorldSubsystems();
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
        
        public async UniTask Activate()
        {
            if (GameWorld == null)
            {
                Debug.LogError("Cannot activate uninitialized WorldContext");
                return;
            }
        
            // 加载场景
            // await LoadScene(Settings.sceneName);
        
            // 创建GameMode
            // GameWorld.CreateGameMode();
        
            // 开始游戏
            // GameWorld.StartGame();
        }
    
        public async UniTask Deactivate()
        {
            if (GameWorld?.GameMode != null)
            {
                await GameWorld.GameMode.EndGame();
            }
        
            //卸载场景
            await UnloadScene();
        }
    
        public T GetSubsystem<T>() where T : XIWorldSubsystem
        {
            return _worldSubsystems.OfType<T>().FirstOrDefault();
        }
    
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
            GameWorld?.Shutdown();
            GameWorld = null;
        }
    
        private async UniTask LoadScene(string sceneName)
        {
            Debug.Log($"Loading scene: {sceneName}");
            // 实际场景加载逻辑
            await UniTask.Delay(500); // 模拟场景加载
            Debug.Log($"Scene loaded: {sceneName}");
        }
    
        private async UniTask UnloadScene()
        {
            Debug.Log("Unloading scene");
            // 实际场景卸载逻辑
            await UniTask.Delay(200); // 模拟场景卸载
            Debug.Log("Scene unloaded");
        }
        
    }
}
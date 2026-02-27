using UnityEngine;
using Cysharp.Threading.Tasks;
using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 游戏引擎入口点 - 唯一的MonoBehaviour启动器
    /// 负责创建GameInstance并驱动其生命周期
    /// </summary>
    public class GameEngine : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private GameInstanceConfiguration _gameInstanceConfig;
        
        /// <summary>
        /// 全局访问点
        /// </summary>
        public static GameEngine Instance { get; private set; }
        
        /// <summary>
        /// 当前游戏实例
        /// </summary>
        public GameInstance GameInstance { get; private set; }
        
        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool IsStarted { get; private set; }
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private async void Start()
        {
            await StartGame();
        }
        
        private void Update()
        {
            if (IsStarted)
            {
                GameInstance?.Update(Time.deltaTime);
            }
        }
        
        private void OnDestroy()
        {
            ShutdownGame();
            
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// 启动游戏 - 创建GameInstance并初始化主世界
        /// </summary>
        public async UniTask StartGame()
        {
            if (IsStarted)
            {
                Debug.LogWarning("[GameEngine] Game already started");
                return;
            }
            
            Debug.Log("[GameEngine] Starting game...");
            
            // 1. 加载配置
            LoadConfiguration();
            
            // 2. 创建并初始化GameInstance
            GameInstance = CreateGameInstance();
            GameInstance.Initialize(_gameInstanceConfig);
            
            // 3. 初始化主世界
            await InitializeMainWorld();
            
            IsStarted = true;
            Debug.Log("[GameEngine] Game started successfully");
        }
        
        /// <summary>
        /// 关闭游戏
        /// </summary>
        public void ShutdownGame()
        {
            if (!IsStarted) return;
            
            Debug.Log("[GameEngine] Shutting down game...");
            
            GameInstance?.Shutdown();
            GameInstance = null;
            IsStarted = false;
            
            Debug.Log("[GameEngine] Game shutdown complete");
        }
        
        /// <summary>
        /// 获取活动世界上下文
        /// </summary>
        public IWorldContext GetActiveWorldContext()
        {
            return GameInstance?.ActiveWorldContext;
        }
        
        #endregion
        
        #region Protected Virtual Methods
        
        /// <summary>
        /// 创建GameInstance实例
        /// 子类可重写以使用自定义GameInstance类型
        /// </summary>
        protected virtual GameInstance CreateGameInstance()
        {
            return new GameInstance();
        }
        
        /// <summary>
        /// 初始化主世界
        /// 子类可重写以自定义主世界的创建逻辑
        /// </summary>
        protected virtual async UniTask InitializeMainWorld()
        {
            if (_gameInstanceConfig?.defaultWorldSettings == null)
            {
                Debug.LogWarning("[GameEngine] No default world settings configured, skipping main world creation");
                return;
            }
            
            var mainWorldSettings = _gameInstanceConfig.defaultWorldSettings;
            
            // 创建主世界上下文
            GameInstance.CreateWorldContext("MainWorld", mainWorldSettings);
            
            // 激活主世界
            GameInstance.SetActiveWorldContext("MainWorld");
            
            await UniTask.Yield();
            
            Debug.Log("[GameEngine] Main world initialized and activated");
        }
        
        #endregion
        
        #region Private Methods
        
        private void LoadConfiguration()
        {
            if (_gameInstanceConfig == null)
            {
                _gameInstanceConfig = Resources.Load<GameInstanceConfiguration>("GameInstance/DefaultGameInstanceConfig");
                
                if (_gameInstanceConfig == null)
                {
                    Debug.LogError("[GameEngine] No GameInstanceConfiguration found! Please assign one in the inspector or place it at Resources/GameInstance/DefaultGameInstanceConfig");
                }
            }
        }
        
        #endregion
    }
}

using UnityEngine;
using UnityEngine.Serialization;

namespace XIFramework.GameFramework
{
// GameInstance配置
    [CreateAssetMenu(fileName = "GameInstanceConfig", menuName = "XIFramework/Game/GameInstance Configuration")]
    public class GameInstanceConfiguration : ScriptableObject, IGameInstanceConfiguration
    {
        [Header("Game Mode")] 
        [SerializeField] 
        [TypeConstraint(typeof(GameMode), IncludeEditorAssemblies = false)]
        public TypeReference _defaultGameMode;
        
        [Header("Basic Settings")]
        public int maxPlayers = 4;
        public bool enableDebug = true;
        
        public int MaxPlayers => maxPlayers;
        public System.Type DefaultGameMode => _defaultGameMode.Type;
        public IWorldSettings DefaultWorldSettings => defaultWorldSettings;
        public bool EnableDebug => enableDebug;
        [Header("World Settings")]
        public WorldSettings defaultWorldSettings;
        
        [Header("Player")] 
        [SerializeField] 
        [TypeConstraint(typeof(PlayerController), AllowAbstract = false, IncludeEditorAssemblies = false)]
        public TypeReference defaultPlayerController; 
        
        public System.Type DefaultPlayerController => defaultPlayerController.Type;
        
        [Header("Networking")]
        public bool enableNetworking;
        public string defaultServerAddress = "127.0.0.1";
        public int defaultServerPort = 7777;
    
        private void OnValidate()
        {
            // if (_defaultGameMode.Type != null && !typeof(GameMode).IsAssignableFrom(_defaultGameMode.Type))
            // {
            //     Debug.LogError("defaultGameMode 属性 不继承自GameMode 自动变更为DefaultGameMode");
            //     _defaultGameMode = new TypeReference(typeof(DefaultGameMode));
            // }
        
            // if (defaultPlayerController.Type != null && !typeof(PlayerController).IsAssignableFrom(defaultPlayerController.Type))
            // {
            //     Debug.LogError("defaultPlayerController 属性不继承自 PlayerController 自动变更为默认控制器");
            //     defaultPlayerController = new TypeReference(typeof(PlayerController));
            // }
        }
    }
}
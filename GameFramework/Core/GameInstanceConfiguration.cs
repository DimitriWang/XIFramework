using UnityEngine;

namespace XIFramework.GameFramework
{
// GameInstance配置
    [CreateAssetMenu(fileName = "GameInstanceConfig", menuName = "XIFramework/Game/GameInstance Configuration")]
    public class GameInstanceConfiguration : ScriptableObject
    {
        [Header("World Settings")]
        public XIWorldSettings defaultWorldSettings;

        [Header("Game Mode")] 
        [SerializeField] 
        [TypeConstraint(typeof(GameMode), IncludeEditorAssemblies = false)]
        public TypeReference _defaultGameMode;

        public System.Type DefaultGameMode => _defaultGameMode.Type;
        
        
        [Header("Core Systems")]
        [SerializeField]
        [TypeConstraint(typeof(GameInstance), IncludeEditorAssemblies = false)]
        private TypeReference _defaultGameInstanceType;
    
        public System.Type DefaultGameInstanceType => _defaultGameInstanceType.Type;
    
        [Space]
        [SerializeField]
        [TypeConstraint(typeof(GameInstance), AllowAbstract = false, IncludeEditorAssemblies = false)]
        private TypeReference _fallbackGameInstanceType;
    
        public System.Type FallbackGameInstanceType => _fallbackGameInstanceType.Type;
        
        [Header("Player")]
      //  public System.Type defaultPlayerController = typeof(PlayerController);
        public int maxPlayers = 4;
    
        [Header("Networking")]
        public bool enableNetworking;
        public string defaultServerAddress = "127.0.0.1";
        public int defaultServerPort = 7777;
    
        private void OnValidate()
        {
            if (!typeof(GameMode).IsAssignableFrom(_defaultGameMode))
            {
                Debug.LogError("defaultGameMode 属性 不继承自GameMode 自动变更为DefaultGameMode");
                _defaultGameMode = typeof(DefaultGameMode);
            }
        
            // if (!typeof(PlayerController).IsAssignableFrom(defaultPlayerController))
            // {
            //     Debug.LogError("defaultPlayerController must be a subclass of PlayerController");
            //     defaultPlayerController = typeof(PlayerController);
            // }
        }
    }
}
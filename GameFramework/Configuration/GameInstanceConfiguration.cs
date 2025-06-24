using UnityEngine;
using UnityEngine.Serialization;

namespace XIFramework.GameFramework
{
// GameInstance配置
    [CreateAssetMenu(fileName = "GameInstanceConfig", menuName = "XIFramework/Game/GameInstance Configuration")]
    public class GameInstanceConfiguration : ScriptableObject
    {
        [Header("Game Mode")] 
        [SerializeField] 
        [TypeConstraint(typeof(XIGameMode), IncludeEditorAssemblies = false)]
        public TypeReference _defaultGameMode;
        
        [Header("World Settings")]
        public XIWorldSettings defaultWorldSettings;

        public System.Type DefaultGameMode => _defaultGameMode.Type;
        
        
        [Header("Core Systems")]
        [SerializeField]
        [TypeConstraint(typeof(XIGameInstance), IncludeEditorAssemblies = false)]
        private TypeReference _defaultGameInstanceType;
    
        public System.Type DefaultGameInstanceType => _defaultGameInstanceType.Type;
    
        [Space]
        [SerializeField]
        [TypeConstraint(typeof(XIGameInstance), AllowAbstract = false, IncludeEditorAssemblies = false)]
        private TypeReference _overrideGameInstanceType;
    
        public System.Type OverrideGameInstanceType => _overrideGameInstanceType.Type;
        
        [Header("Player")]
      //  public System.Type defaultPlayerController = typeof(PlayerController);
        public int maxPlayers = 4;
    
        [Header("Networking")]
        public bool enableNetworking;
        public string defaultServerAddress = "127.0.0.1";
        public int defaultServerPort = 7777;
    
        private void OnValidate()
        {
            if (!typeof(XIGameMode).IsAssignableFrom(_defaultGameMode))
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
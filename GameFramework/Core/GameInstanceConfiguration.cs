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
        public System.Type defaultGameMode = typeof(DefaultGameMode);
    
        [Header("Player")]
      //  public System.Type defaultPlayerController = typeof(PlayerController);
        public int maxPlayers = 4;
    
        [Header("Networking")]
        public bool enableNetworking;
        public string defaultServerAddress = "127.0.0.1";
        public int defaultServerPort = 7777;
    
        private void OnValidate()
        {
            if (!typeof(GameMode).IsAssignableFrom(defaultGameMode))
            {
                Debug.LogError("defaultGameMode must be a subclass of GameMode");
                defaultGameMode = typeof(DefaultGameMode);
            }
        
            // if (!typeof(PlayerController).IsAssignableFrom(defaultPlayerController))
            // {
            //     Debug.LogError("defaultPlayerController must be a subclass of PlayerController");
            //     defaultPlayerController = typeof(PlayerController);
            // }
        }
    }
}
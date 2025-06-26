using UnityEngine;

namespace XIFramework.GameFramework
{
    public class DefaultGameMode : XIGameMode
    {
        public override void Initialize(XIGameWorld world)
        {
            DefaultPlayerControllerType = typeof(DefaultPlayerController);
        }

        public override void StartGame()
        {
            base.StartGame();
            Debug.Log("Default Game Mode started");
        }
    }
}
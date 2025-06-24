using XIFramework.GameFramework;
using UnityEngine;

namespace XIFramework.GameLaunch
{

    [CreateAssetMenu(fileName = "UIFeature", menuName = "Game Features/UI Feature")]
    public class UIFeature : XIGameFeature
    {
        public override void Initialize()
        {
         
            Debug.Log("UIFeature Init");
        }
        public override void UpdateFeature(float deltaTime)
        {
        }
        public override void Shutdown()
        {
        }
    }
}
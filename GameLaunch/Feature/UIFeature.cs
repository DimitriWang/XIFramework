using XIFramework.GameFramework;
using UnityEngine;

namespace XIFramework.GameLaunch
{

    [CreateAssetMenu(fileName = "UIFeature", menuName = "Game Features/UI Feature")]
    public class UIFeature : XIGameFeature
    {
        public override void Initialize()
        {
            
            RegisterSubsystem<UISubsystem>();
        }
        public override void Update()
        {
        }
        public override void Shutdown()
        {
        }
    }
}
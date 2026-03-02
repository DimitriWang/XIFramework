using XIFramework.GameFramework;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 默认游戏实例 - 可在此处添加项目特有的初始化逻辑
    /// </summary>
    public class DefaultGameInstance : GameInstance
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            // 在此注册项目特有的全局服务
        }
    }
}

namespace XIFramework.GameFramework
{
    public class GameMode
    {
        public virtual void StartGame()
        {
            
        }
    }
    
    // 默认GameMode实现
    public class DefaultGameMode : GameMode
    {
        public override void StartGame()
        {
            base.StartGame();
        }
    }
}
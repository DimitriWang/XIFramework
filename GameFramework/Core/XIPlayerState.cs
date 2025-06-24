namespace XIFramework.GameFramework
{
    public class XIPlayerState
    {
        public XIGameWorld World { get; private set; }
        public int PlayerId { get; private set; }
        public int Score { get; set; }
        public bool IsReady { get; set; }
    
        public virtual void Initialize(XIGameWorld world, int playerId)
        {
            World = world;
            PlayerId = playerId;
        }
    
        public virtual void Reset()
        {
            Score = 0;
            IsReady = false;
        }
    }
}
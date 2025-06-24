namespace XIFramework.GameFramework
{
    using System.Collections.Generic;

    public class XIGameState
    {
        [Inject]
        public XIGameWorld World { get; protected set; }
        public bool IsGameActive { get; protected set; }
        public float GameTime { get; protected set; }
    
        private readonly List<XIPlayerState> _playerStates = new();
    
        public IReadOnlyList<XIPlayerState> PlayerStates => _playerStates;
    
        public virtual void Initialize(XIGameWorld world)
        {
            World = world;
        }
    
        public virtual void StartGame()
        {
            IsGameActive = true;
            GameTime = 0f;
        }
    
        public virtual void EndGame()
        {
            IsGameActive = false;
        }
    
        public virtual void Update(float deltaTime)
        {
            if (IsGameActive)
            {
                GameTime += deltaTime;
            }
        }
    
        public void AddPlayerState(XIPlayerState playerState)
        {
            _playerStates.Add(playerState);
        }
    
        public XIPlayerState GetPlayerState(int playerId)
        {
            return _playerStates.Find(p => p.PlayerId == playerId);
        }
    }
}
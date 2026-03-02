using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏状态实现类 - 管理游戏全局状态
    /// </summary>
    public class GameState : IGameState
    {
        #region Properties
        
        public IGameWorld World { get; private set; }
        public bool IsGameActive { get; private set; }
        public float GameTime { get; private set; }
        public IReadOnlyList<IPlayerState> PlayerStates => _playerStates.AsReadOnly();
        
        #endregion
        
        #region Private Fields
        
        private readonly List<IPlayerState> _playerStates = new List<IPlayerState>();
        
        #endregion
        
        #region IGameState Implementation
        
        public virtual void Initialize(IGameWorld world)
        {
            World = world;
            IsGameActive = false;
            GameTime = 0f;
            
            Debug.Log($"[GameState] Initialized in world {World.Name}");
        }
        
        public virtual void StartGame()
        {
            IsGameActive = true;
            GameTime = 0f;
            
            Debug.Log($"[GameState] Game started in world {World.Name}");
        }
        
        public virtual void EndGame()
        {
            IsGameActive = false;
            
            Debug.Log($"[GameState] Game ended in world {World.Name}");
        }
        
        public virtual void Update(float deltaTime)
        {
            if (!IsGameActive) return;
            
            GameTime += deltaTime;
            
            // 更新所有玩家状态
            foreach (var playerState in _playerStates)
            {
                playerState.Update(deltaTime);
            }
        }
        
        public void AddPlayerState(IPlayerState playerState)
        {
            if (playerState == null || _playerStates.Contains(playerState))
                return;
            
            _playerStates.Add(playerState);
            Debug.Log($"[GameState] Added player state for player {playerState.PlayerId}");
        }
        
        public void RemovePlayerState(IPlayerState playerState)
        {
            if (_playerStates.Remove(playerState))
            {
                Debug.Log($"[GameState] Removed player state for player {playerState.PlayerId}");
            }
        }
        
        public IPlayerState GetPlayerState(int playerId)
        {
            return _playerStates.FirstOrDefault(p => p.PlayerId == playerId);
        }
        
        #endregion
    }
}
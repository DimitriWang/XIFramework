using UnityEngine;

namespace XIFramework.GameLaunch
{
    /// <summary>
    /// 游戏引导器 - 确保GameEngine存在
    /// 可放在任何场景中，如果GameEngine不存在则自动创建
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameEngine _gameEnginePrefab;
        
        private void Awake()
        {
            // 如果GameEngine已存在，无需重复创建
            if (GameEngine.Instance != null)
            {
                Debug.Log("[GameBootstrap] GameEngine already exists, skipping");
                return;
            }
            
            // 创建GameEngine
            if (_gameEnginePrefab != null)
            {
                _gameEnginePrefab = Instantiate(_gameEnginePrefab);
                Debug.Log("[GameBootstrap] Created GameEngine from prefab");
            }
            else
            {
                var go = new GameObject("GameEngine");
                _gameEnginePrefab = go.AddComponent<GameEngine>();
                Debug.Log("[GameBootstrap] Created default GameEngine");
            }
        }
    }
}

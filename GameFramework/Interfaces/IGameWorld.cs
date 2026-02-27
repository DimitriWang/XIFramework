using System;
using System.Collections.Generic;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 游戏世界接口 - 继承自IWorld，提供游戏专用功能
    /// 对应Unreal中的游戏世界概念
    /// </summary>
    public interface IGameWorld : IWorld
    {
        /// <summary>
        /// 获取世界上下文
        /// </summary>
        IWorldContext Context { get; }
        
        /// <summary>
        /// 获取游戏模式
        /// </summary>
        IGameMode GameMode { get; }
        
        /// <summary>
        /// 获取游戏状态
        /// </summary>
        IGameState GameState { get; }
        
        /// <summary>
        /// 是否正在运行
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// 当前激活的持久关卡
        /// </summary>
        string ActivePersistentLevel { get; }
        
        /// <summary>
        /// 关卡是否已初始化
        /// </summary>
        bool IsLevelsInitialized { get; }
        
        /// <summary>
        /// 游戏是否已开始
        /// </summary>
        bool IsGameStarted { get; }
        
        /// <summary>
        /// 已加载的子关卡列表
        /// </summary>
        IReadOnlyList<string> LoadedSubLevels { get; }
        
        /// <summary>
        /// 加载持久关卡
        /// </summary>
        /// <param name="levelName">关卡名称</param>
        void LoadPersistentLevel(string levelName);
        
        /// <summary>
        /// 加载子关卡
        /// </summary>
        /// <param name="levelName">关卡名称</param>
        void LoadSubLevel(string levelName);
        
        /// <summary>
        /// 卸载关卡
        /// </summary>
        /// <param name="levelName">关卡名称</param>
        void UnloadLevel(string levelName);
        
        /// <summary>
        /// 初始化关卡系统
        /// </summary>
        void InitializeLevels();
        
        /// <summary>
        /// 启动游戏
        /// </summary>
        void StartGame();
        
        /// <summary>
        /// 结束游戏
        /// </summary>
        void EndGame();
    }
}
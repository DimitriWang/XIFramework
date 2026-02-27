using System;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 世界接口 - 最底层的抽象，代表一个独立的世界环境
    /// 对应Unreal中的UWorld概念
    /// </summary>
    public interface IWorld
    {
        /// <summary>
        /// 世界名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 世界唯一标识符
        /// </summary>
        Guid WorldId { get; }
        
        /// <summary>
        /// 世界当前状态
        /// </summary>
        WorldState State { get; }
        
        /// <summary>
        /// 世界时间（秒）
        /// </summary>
        float Time { get; }
        
        /// <summary>
        /// 世界DeltaTime
        /// </summary>
        float DeltaTime { get; }
        
        /// <summary>
        /// 初始化世界
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 激活世界
        /// </summary>
        void Activate();
        
        /// <summary>
        /// 更新世界逻辑
        /// </summary>
        /// <param name="deltaTime">帧间隔时间</param>
        void Update(float deltaTime);
        
        /// <summary>
        /// 停用世界
        /// </summary>
        void Deactivate();
        
        /// <summary>
        /// 关闭世界
        /// </summary>
        void Shutdown();
        
        /// <summary>
        /// 世界状态枚举
        /// </summary>
        public enum WorldState
        {
            /// <summary>
            /// 未初始化
            /// </summary>
            Uninitialized,
            
            /// <summary>
            /// 已初始化但未激活
            /// </summary>
            Initialized,
            
            /// <summary>
            /// 激活状态
            /// </summary>
            Active,
            
            /// <summary>
            /// 已停用
            /// </summary>
            Inactive,
            
            /// <summary>
            /// 已关闭
            /// </summary>
            Shutdown
        }
    }
}
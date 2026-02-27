using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace XIFramework.GameFramework
{
    /// <summary>
    /// 子系统管理器 - 统一管理子系统的注册、初始化和生命周期
    /// </summary>
    public class XISubSystemManager
    {
        private readonly Dictionary<Type, ISubsystem> _subsystems = new();
        private readonly List<ISubsystem> _subSystemList = new();
        private readonly IXIFrameworkContainer _container;
        
        public XISubSystemManager(IXIFrameworkContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }
        
        /// <summary>
        /// 注册子系统（通过Type）
        /// 通过容器创建实例，自动注入依赖，并调用Initialize
        /// </summary>
        public void RegisterSubSystem(Type subsystemType)
        {
            if (!typeof(ISubsystem).IsAssignableFrom(subsystemType))
                throw new ArgumentException($"Type {subsystemType.Name} does not implement ISubsystem");
            
            if (_subsystems.ContainsKey(subsystemType))
            {
                Debug.LogWarning($"[XISubSystemManager] {subsystemType.Name} already registered");
                return;
            }
            
            // 通过容器创建实例（自动构造函数注入+属性注入）
            var subsystem = (ISubsystem)_container.Resolve(subsystemType);
            _container.Inject(subsystem);
            
            _subsystems[subsystemType] = subsystem;
            _subSystemList.Add(subsystem);
            
            // 自动初始化
            subsystem.Initialize();
            
            Debug.Log($"[XISubSystemManager] Registered and initialized: {subsystemType.Name}");
        }
        
        /// <summary>
        /// 注册子系统（泛型）
        /// </summary>
        public void RegisterSubSystem<T>() where T : class, ISubsystem
        {
            RegisterSubSystem(typeof(T));
        }
        
        /// <summary>
        /// 获取子系统
        /// </summary>
        public T GetSubsystem<T>() where T : class, ISubsystem
        {
            return _subsystems.TryGetValue(typeof(T), out var subsystem) ? (T)subsystem : default;
        }
        
        /// <summary>
        /// 更新所有子系统
        /// </summary>
        public void UpdateSubSystems(float deltaTime)
        {
            for (int i = 0; i < _subSystemList.Count; i++)
            {
                _subSystemList[i]?.Update(deltaTime);
            }
        }
        
        /// <summary>
        /// 关闭所有子系统（支持异步关闭）
        /// </summary>
        public async UniTask ShutdownAll()
        {
            // 逆序关闭
            for (int i = _subSystemList.Count - 1; i >= 0; i--)
            {
                var subsystem = _subSystemList[i];
                try
                {
                    if (subsystem is IAsyncShutdown asyncShutdown)
                    {
                        await asyncShutdown.ShutdownAsync();
                    }
                    else
                    {
                        subsystem.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[XISubSystemManager] Error shutting down {subsystem.GetType().Name}: {ex.Message}");
                }
            }
            
            _subsystems.Clear();
            _subSystemList.Clear();
        }
        
        /// <summary>
        /// 同步关闭所有子系统
        /// </summary>
        public void ShutdownAllSync()
        {
            for (int i = _subSystemList.Count - 1; i >= 0; i--)
            {
                try
                {
                    _subSystemList[i].Shutdown();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[XISubSystemManager] Error shutting down: {ex.Message}");
                }
            }
            
            _subsystems.Clear();
            _subSystemList.Clear();
        }
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace XIFramework.GameFramework
{
    public static partial class XIGameFramework
    {
        public enum FrameworkStatus
        {
            NotInitialized,
            Initializing,
            Running,
            ShuttingDown
        }
        private static FrameworkStatus _status = FrameworkStatus.NotInitialized;
        private static XIGameInstance _gameInstance;
        private static readonly List<Type> _preInitSystems = new();
        private static readonly List<Type> _postInitSystems = new();
        public static FrameworkStatus Status => _status;
        public static async UniTask Initialize(XIGameInstance xiGameInstance)
        {
            if (_status != FrameworkStatus.NotInitialized)
                throw new InvalidOperationException("Framework already initialized");
            _status = FrameworkStatus.Initializing;
            _gameInstance = xiGameInstance;

            // 执行预初始化系统
            await RunInitializationSystems(_preInitSystems);

            // 初始化游戏实例
            //await _gameInstance.Initialize();

            // 执行后初始化系统
            await RunInitializationSystems(_postInitSystems);
            _status = FrameworkStatus.Running;

            // 发送框架初始化完成事件
            SendEvent(new FrameworkInitializedEvent());
        }
        private static async UniTask RunInitializationSystems(List<Type> systemTypes)
        {
            foreach (var type in systemTypes)
            {
                var system = (ICustomInitializationSystem)Activator.CreateInstance(type);
                await system.Initialize();
            }
        }
        public static void AddPreInitSystem<T>() where T : ICustomInitializationSystem
        {
            if (_status != FrameworkStatus.NotInitialized)
                throw new InvalidOperationException("Cannot add systems after initialization has started");
            _preInitSystems.Add(typeof(T));
        }
        public static void AddPostInitSystem<T>() where T : ICustomInitializationSystem
        {
            if (_status != FrameworkStatus.NotInitialized)
                throw new InvalidOperationException("Cannot add systems after initialization has started");
            _postInitSystems.Add(typeof(T));
        }
        public static T GetSystem<T>() where T : XIGameSubSystem
        {
            if (_gameInstance == null)
                throw new InvalidOperationException("GameInstance not initialized");
            // return _gameInstance.GetSubsystem<T>();
            return null;
        }
        public static async UniTask Shutdown()
        {
            await UniTask.CompletedTask;
            if (_status != FrameworkStatus.Running)
                return;
            _status = FrameworkStatus.ShuttingDown;

            // 发送框架关闭事件
            SendEvent(new FrameworkShutdownEvent());
           // await _gameInstance.Shutdown();
            _gameInstance = null;
            _status = FrameworkStatus.NotInitialized;
        }

        // QFramework风格事件发送
        public static void SendEvent<T>(T eventData) where T : struct
        {
            //var eventSystem = GetSystem<EventSubsystem>();
            //eventSystem?.SendEvent(eventData);
        }
    }

// 自定义初始化系统接口
    public interface ICustomInitializationSystem
    {
        UniTask Initialize();
    }

// 框架事件
    public struct FrameworkInitializedEvent { }
    public struct FrameworkShutdownEvent { }
}
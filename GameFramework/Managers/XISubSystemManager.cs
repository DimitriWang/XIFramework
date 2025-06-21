using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace XIFramework.GameFramework
{
    public class XISubSystemManager
    {
        private readonly Dictionary<Type, XIGameSubSystem> _subsystems = new();
        private readonly List<XIGameSubSystem> _subSystemList = new();
        private readonly XIFrameworkContainer _framework;
        public XISubSystemManager(XIFrameworkContainer framework)
        {
            _framework = framework;
        }
        public void RegisterSubSystem(Type subsystemType)
        {
            if (!typeof(XIGameSubSystem).IsAssignableFrom(subsystemType))
                throw new ArgumentException($"Type {subsystemType} is not a GameSubSystem");
            if (_subsystems.ContainsKey(subsystemType))
                return;
            var subsystem = (XIGameSubSystem)Activator.CreateInstance(subsystemType);
            _framework.Inject(subsystem);
            _subsystems[subsystemType] = subsystem;
            _subSystemList.Add(subsystem);
        }
        public void RegisterSubSystem<T>() where T : XIGameSubSystem
        {
            RegisterSubSystem(typeof(T));
        }
        public async UniTask InitializeAll()
        {
            foreach (var subsystem in _subsystems.Values)
            {
                if (subsystem is IAsyncInitialization asyncSubsystem)
                {
                    await asyncSubsystem.InitializeAsync();
                }
                else
                {
                    subsystem.Initialize();
                }
            }
        }
        public async UniTask ShutdownAll()
        {
            foreach (var subsystem in _subsystems.Values)
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
            _subsystems.Clear();
        }
        public T GetSubsystem<T>() where T : XIGameSubSystem
        {
            return _subsystems.TryGetValue(typeof(T), out var subsystem) ? (T)subsystem : null;
        }
        public void UpdateSubSystems(float deltaTime)
        {
            foreach (var subSystem in _subSystemList)
            {
                subSystem?.Update(deltaTime);
            }
        }
    }
}
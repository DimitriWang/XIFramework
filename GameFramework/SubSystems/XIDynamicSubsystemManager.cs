using System.Collections.Generic;

namespace XIFramework.GameFramework
{
    [AutoCreateSubsystem]
    public class XIDynamicSubsystemManager : XIGameInstanceSubsystem
    {
        private readonly Dictionary<System.Type, XIGameInstanceSubsystem> _dynamicSubsystems = new();
    
        public T AddSubsystem<T>() where T : XIGameInstanceSubsystem, new()
        {
            var subsystem = new T();
            subsystem.GameInstance = GameInstance;
            subsystem.Initialize();
            _dynamicSubsystems[typeof(T)] = subsystem;
            return subsystem;
        }
    
        public bool RemoveSubsystem<T>() where T : XIGameInstanceSubsystem
        {
            if (_dynamicSubsystems.TryGetValue(typeof(T), out var subsystem))
            {
                subsystem.Shutdown();
                return _dynamicSubsystems.Remove(typeof(T));
            }
            return false;
        }
    
        public override void Update(float deltaTime)
        {
            foreach (var subsystem in _dynamicSubsystems.Values)
            {
                subsystem.Update(deltaTime);
            }
        }
    
        public override void Shutdown()
        {
            foreach (var subsystem in _dynamicSubsystems.Values)
            {
                subsystem.Shutdown();
            }
            _dynamicSubsystems.Clear();
        }
    }
}
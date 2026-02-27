using System;
using System.Collections.Generic;
using UnityEngine;

namespace XIFramework
{
    /// <summary>
    /// 服务定位器实现 - 简化版的IOC容器
    /// 避免过度依赖注入，提供必要的解耦功能
    /// </summary>
    public class ServiceLocator : IServiceLocator
    {
        #region Singleton
        
        private static ServiceLocator _instance;
        public static ServiceLocator Instance => _instance ?? (_instance = new ServiceLocator());
        
        #endregion
        
        #region Private Fields
        
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();
        
        #endregion
        
        #region IServiceLocator Implementation
        
        public void Register<T>(T instance) where T : class
        {
            var type = typeof(T);
            _services[type] = instance ?? throw new ArgumentNullException(nameof(instance));
            
            Debug.Log($"[ServiceLocator] Registered service: {type.Name}");
        }
        
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImplementation);
            
            _typeMappings[interfaceType] = implementationType;
            
            Debug.Log($"[ServiceLocator] Registered mapping: {interfaceType.Name} -> {implementationType.Name}");
        }
        
        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            
            // 1. 检查是否有直接注册的实例
            if (_services.TryGetValue(type, out var service))
            {
                return (T)service;
            }
            
            // 2. 检查是否有类型映射
            if (_typeMappings.TryGetValue(type, out var implementationType))
            {
                var instance = CreateInstance(implementationType);
                _services[type] = instance;
                return (T)instance;
            }
            
            // 3. 尝试直接创建实例
            if (!type.IsInterface && !type.IsAbstract)
            {
                var instance = CreateInstance(type);
                _services[type] = instance;
                return (T)instance;
            }
            
            throw new InvalidOperationException($"No service registered for type {type.Name}");
        }
        
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            var type = typeof(T);
            var results = new List<T>();
            
            // 查找所有匹配的服务
            foreach (var kvp in _services)
            {
                if (type.IsAssignableFrom(kvp.Key))
                {
                    results.Add((T)kvp.Value);
                }
            }
            
            return results;
        }
        
        public bool IsRegistered<T>() where T : class
        {
            var type = typeof(T);
            return _services.ContainsKey(type) || _typeMappings.ContainsKey(type);
        }
        
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            _services.Remove(type);
            _typeMappings.Remove(type);
            
            Debug.Log($"[ServiceLocator] Unregistered service: {type.Name}");
        }
        
        #endregion
        
        #region Private Methods
        
        private object CreateInstance(Type type)
        {
            try
            {
                // 尝试使用无参构造函数
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create instance of {type.Name}", ex);
            }
        }
        
        #endregion
        
        #region Public Static Methods
        
        public static void RegisterService<T>(T instance) where T : class
        {
            Instance.Register(instance);
        }
        
        public static void RegisterService<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            Instance.Register<TInterface, TImplementation>();
        }
        
        public static T GetService<T>() where T : class
        {
            return Instance.Resolve<T>();
        }
        
        public static IEnumerable<T> GetAllServices<T>() where T : class
        {
            return Instance.ResolveAll<T>();
        }
        
        public static bool HasService<T>() where T : class
        {
            return Instance.IsRegistered<T>();
        }
        
        public static void RemoveService<T>() where T : class
        {
            Instance.Unregister<T>();
        }
        
        #endregion
    }
}
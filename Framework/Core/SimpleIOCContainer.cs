using System;
using System.Collections.Generic;
using UnityEngine;

namespace XIFramework
{
    /// <summary>
    /// 简化版IOC容器接口
    /// </summary>
    public interface ISimpleIOCContainer
    {
        /// <summary>
        /// 注册实例
        /// </summary>
        void Register<T>(T instance) where T : class;
        
        /// <summary>
        /// 注册类型映射
        /// </summary>
        void Register<TInterface, TImplementation>() where TImplementation : TInterface, new();
        
        /// <summary>
        /// 解析服务
        /// </summary>
        T Resolve<T>() where T : class;
        
        /// <summary>
        /// 解析所有实例
        /// </summary>
        IEnumerable<T> ResolveAll<T>() where T : class;
        
        /// <summary>
        /// 检查是否已注册
        /// </summary>
        bool IsRegistered<T>() where T : class;
    }
    
    /// <summary>
    /// 简化版IOC容器实现
    /// </summary>
    public class SimpleIOCContainer : ISimpleIOCContainer
    {
        #region Singleton Pattern
        
        private static SimpleIOCContainer _instance;
        public static SimpleIOCContainer Instance => _instance ??= new SimpleIOCContainer();
        
        #endregion
        
        #region Private Fields
        
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();
        
        #endregion
        
        #region ISimpleIOCContainer Implementation
        
        public void Register<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
                
            var type = typeof(T);
            _instances[type] = instance;
            
            Debug.Log($"[SimpleIOC] Registered instance: {type.Name}");
        }
        
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            var interfaceType = typeof(TInterface);
            var implementationType = typeof(TImplementation);
            
            _typeMappings[interfaceType] = implementationType;
            
            Debug.Log($"[SimpleIOC] Registered mapping: {interfaceType.Name} -> {implementationType.Name}");
        }
        
        public T Resolve<T>() where T : class
        {
            var type = typeof(T);
            
            // 1. 检查是否有直接注册的实例
            if (_instances.TryGetValue(type, out var instance))
            {
                return (T)instance;
            }
            
            // 2. 检查类型映射
            if (_typeMappings.TryGetValue(type, out var implType))
            {
                var newInstance = CreateInstance(implType);
                _instances[type] = newInstance;
                return (T)newInstance;
            }
            
            // 3. 尝试直接创建实例
            if (!type.IsInterface && !type.IsAbstract)
            {
                var newInstance = CreateInstance(type);
                _instances[type] = newInstance;
                return (T)newInstance;
            }
            
            throw new InvalidOperationException($"No registration found for type {type.Name}");
        }
        
        public IEnumerable<T> ResolveAll<T>() where T : class
        {
            var type = typeof(T);
            var results = new List<T>();
            
            // 查找所有匹配的实例
            foreach (var kvp in _instances)
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
            return _instances.ContainsKey(type) || _typeMappings.ContainsKey(type);
        }
        
        #endregion
        
        #region Private Methods
        
        private object CreateInstance(Type type)
        {
            try
            {
                // 尝试使用无参构造函数创建实例
                var instance = Activator.CreateInstance(type);
                Debug.Log($"[SimpleIOC] Created instance: {type.Name}");
                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create instance of {type.Name}: {ex.Message}");
            }
        }
        
        #endregion
        
        #region Static Helper Methods
        
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
        
        #endregion
    }
}
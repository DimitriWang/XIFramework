// InjectAttribute.cs 保持不变

// IOCKit.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XIFramework
{
    public interface IXIFrameworkContainer
    {
        void Register<T>(T instance, bool isSingleton = true) where T : class;
        void Register<TInterface, TImplementation>(bool isSingleton = true) where TImplementation : TInterface;
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        void Inject(object obj);
        IXIFrameworkContainer CreateChildContainer();
        
        // 新增方法
        bool IsRegistered(Type type);
        void RegisterInstance(Type type, object instance);
    }

    public class XIFrameworkContainer : IXIFrameworkContainer
    {
        private readonly Dictionary<Type, object> _singletonInstances = new();
        private readonly Dictionary<Type, Type> _interfaceMappings = new();
        private readonly Dictionary<Type, bool> _singletonRegistrations = new();
        private readonly IXIFrameworkContainer _parentContainer;
        private readonly HashSet<Type> _resolvingStack = new(); // 防止循环依赖

        public XIFrameworkContainer(IXIFrameworkContainer parentContainer = null)
        {
            _parentContainer = parentContainer;
        }

        public void Register<T>(T instance, bool isSingleton = true) where T : class
        {
            var type = typeof(T);
            if (isSingleton)
            {
                _singletonInstances[type] = instance;
                _singletonRegistrations[type] = true;
            }
            else
            {
                // 瞬态对象不缓存实例，但注册类型
                _singletonRegistrations[type] = false;
            }
        }

        public void Register<TInterface, TImplementation>(bool isSingleton = true) where TImplementation : TInterface
        {
            var interfaceType = typeof(TInterface);
            var implType = typeof(TImplementation);
            
            _interfaceMappings[interfaceType] = implType;
            _singletonRegistrations[interfaceType] = isSingleton;
        }

        public void RegisterInstance(Type type, object instance)
        {
            if (!type.IsInstanceOfType(instance))
                throw new ArgumentException($"Instance is not of type {type.Name}");
            
            _singletonInstances[type] = instance;
            _singletonRegistrations[type] = true;
        }

        public bool IsRegistered(Type type)
        {
            if (_singletonInstances.ContainsKey(type)) return true;
            if (_singletonRegistrations.ContainsKey(type)) return true;
            if (_interfaceMappings.ContainsKey(type)) return true;
            
            return _parentContainer?.IsRegistered(type) ?? false;
        }

        public T Resolve<T>() where T : class => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            // 防止循环依赖
            if (_resolvingStack.Contains(type))
                throw new InvalidOperationException($"Circular dependency detected for type {type.Name}");
            
            _resolvingStack.Add(type);
            
            try
            {
                // 1. 检查单例实例缓存
                if (_singletonInstances.TryGetValue(type, out var singleton))
                    return singleton;

                // 2. 检查接口映射
                if (type.IsInterface && _interfaceMappings.TryGetValue(type, out var implType))
                {
                    var instance = CreateInstance(implType);
                    
                    // 如果是单例则缓存
                    if (_singletonRegistrations.TryGetValue(type, out var isSingleton) && isSingleton)
                    {
                        _singletonInstances[type] = instance;
                    }
                    
                    return instance;
                }

                // 3. 检查父容器
                if (_parentContainer != null && _parentContainer.IsRegistered(type))
                {
                    return _parentContainer.Resolve(type);
                }

                // 4. 尝试直接创建实例
                if (!type.IsAbstract && !type.IsInterface)
                {
                    return CreateInstance(type);
                }
 
                throw new InvalidOperationException($"No registration found for type {type.Name}");
            }
            finally
            {
                _resolvingStack.Remove(type);
            }
        }

        public void Inject(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();
            
            // 1. 字段注入
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.GetCustomAttribute<InjectAttribute>() != null);
            
            foreach (var field in fields)
            {
                try
                {
                    var value = Resolve(field.FieldType);
                    field.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to inject field {field.Name} in {type.Name}: {ex.Message}");
                }
            }

            // 2. 属性注入
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetCustomAttribute<InjectAttribute>() != null && p.CanWrite);
            
            foreach (var prop in properties)
            {
                try
                {
                    var value = Resolve(prop.PropertyType);
                    prop.SetValue(obj, value);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to inject property {prop.Name} in {type.Name}: {ex.Message}");
                }
            }
        }

        public IXIFrameworkContainer CreateChildContainer()
        {
            return new XIFrameworkContainer(this);
        }

        private object CreateInstance(Type type)
        {
            // 尝试使用构造函数注入
            var ctors = type.GetConstructors();
            if (ctors.Length > 0)
            {
                // 选择参数最多的构造函数
                var ctor = ctors
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault(c => c.GetParameters().All(p => IsRegistered(p.ParameterType)));
                
                if (ctor != null)
                {
                    var parameters = ctor.GetParameters()
                        .Select(p => Resolve(p.ParameterType))
                        .ToArray();
                    
                    var instance = Activator.CreateInstance(type, parameters);
                    Inject(instance); // 对构造函数创建的对象执行字段/属性注入
                    return instance;
                }
            }

            // 回退到无参构造函数
            try
            {
                var instance = Activator.CreateInstance(type);
                Inject(instance);
                return instance;
            }
            catch (MissingMethodException)
            {
                throw new InvalidOperationException(
                    $"No suitable constructor found for {type.Name}. " +
                    "Ensure all dependencies are registered or provide a parameterless constructor.");
            }
        }
    }
}
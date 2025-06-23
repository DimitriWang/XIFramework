using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XIFramework.GameFramework
{
    public interface IXIFrameworkContainer
    {
        void Register<T>(T instance) where T : class;
        void Register<TInterface, TImplementation>() where TImplementation : TInterface;
        T Get<T>() where T : class;
        bool TryGet<T>(out T instance) where T : class;
        void Inject(object obj);
    }
    public class XIFrameworkContainer : IXIFrameworkContainer
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, Type> _interfaceMappings = new();
        public void Register<T>(T instance) where T : class
        {
            _instances[typeof(T)] = instance;
        }
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _interfaceMappings[typeof(TInterface)] = typeof(TImplementation);
        }
        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var instance))
            {
                return instance;
            }
            throw new InvalidOperationException($"No instance of type {typeof(T)} registered");
        }
        public bool TryGet<T>(out T instance) where T : class
        {
            Type type = typeof(T);

            // 直接实例获取
            if (_instances.TryGetValue(type, out object obj))
            {
                instance = (T)obj;
                return true;
            }

            // 接口映射获取
            if (type.IsInterface && _interfaceMappings.TryGetValue(type, out Type implType))
            {
                instance = (T)Get(implType);
                return true;
            }

            // 尝试创建实例
            try
            {
                instance = CreateInstance<T>();
                _instances[type] = instance;
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }
        public void Inject(object obj)
        {
            var fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() != null)
                {
                    var fieldType = field.FieldType;
                    if (TryGet(fieldType, out object value))
                    {
                        field.SetValue(obj, value);
                    }
                }
            }
        }
        private object Get(Type type)
        {
            if (_instances.TryGetValue(type, out object instance))
            {
                return instance;
            }
            if (type.IsInterface && _interfaceMappings.TryGetValue(type, out Type implType))
            {
                return CreateInstance(implType);
            }
            return CreateInstance(type);
        }
        private T CreateInstance<T>() where T : class
        {
            return (T)CreateInstance(typeof(T));
        }
        private object CreateInstance(Type type)
        {
            var ctors = type.GetConstructors();
            if (ctors.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            // 选择参数最多的构造函数
            var ctor = ctors.OrderByDescending(c => c.GetParameters().Length).First();
            var parameters = ctor.GetParameters();
            var paramInstances = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                paramInstances[i] = Get(parameters[i].ParameterType);
            }
            var instance = Activator.CreateInstance(type, paramInstances);
            Inject(instance); // 注入依赖
            return instance;
        }
        private bool TryGet(Type type, out object instance)
        {
            if (_instances.TryGetValue(type, out instance))
            {
                return true;
            }
            if (type.IsInterface && _interfaceMappings.TryGetValue(type, out Type implType))
            {
                instance = Get(implType);
                return true;
            }
            try
            {
                instance = CreateInstance(type);
                _instances[type] = instance;
                return true;
            }
            catch
            {
                instance = null;
                return false;
            }
        }
    }
}
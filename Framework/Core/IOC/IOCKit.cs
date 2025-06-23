using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XIFramework
{
    public interface IXIFrameworkContainer
    {
        void Register<T>(T instance) where T : class;
        void Register<TInterface, TImplementation>() where TImplementation : TInterface;
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        void Inject(object obj);
        IXIFrameworkContainer CreateChildContainer();
    }
    public class XIFrameworkContainer : IXIFrameworkContainer
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, Type> _interfaceMappings = new();
        private readonly IXIFrameworkContainer _parentContainer;
        public XIFrameworkContainer(IXIFrameworkContainer parentContainer = null)
        {
            _parentContainer = parentContainer;
        }
        public void Register<T>(T instance) where T : class
        {
            _instances[typeof(T)] = instance;
        }
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _interfaceMappings[typeof(TInterface)] = typeof(TImplementation);
        }
        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }
        public object Resolve(Type type)
        {
            // 尝试从当前容器解析
            if (_instances.TryGetValue(type, out var instance))
            {
                return instance;
            }

            // 尝试从接口映射解析
            if (type.IsInterface && _interfaceMappings.TryGetValue(type, out var implType))
            {
                return CreateInstance(implType);
            }

            // 尝试从父容器解析
            if (_parentContainer != null)
            {
                try
                {
                    return _parentContainer.Resolve(type);
                }
                catch
                {
                    // 父容器没有注册，继续尝试创建实例
                }
            }

            // 尝试创建实例
            return CreateInstance(type);
        }
        public void Inject(object obj)
        {
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() != null)
                {
                    var fieldType = field.FieldType;
                    var value = Resolve(fieldType);
                    field.SetValue(obj, value);
                }
            }
        }
        public IXIFrameworkContainer CreateChildContainer()
        {
            return new XIFrameworkContainer(this);
        }
        public object CreateInstance(Type type)
        {
            var ctors = type.GetConstructors();
            if (ctors.Length == 0)
            {
                var instance = Activator.CreateInstance(type);
                Inject(instance);
                return instance;
            }
            else
            {
                // 选择参数最多的构造函数
                var ctor = ctors.OrderByDescending(c => c.GetParameters().Length).First();
                var parameters = ctor.GetParameters();
                var paramInstances = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    paramInstances[i] = Resolve(parameters[i].ParameterType);
                }
                var instance = Activator.CreateInstance(type, paramInstances);
                Inject(instance);
                return instance;
            }
        }
    }

}
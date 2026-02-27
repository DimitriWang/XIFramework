using System;
using System.Collections.Generic;

namespace XIFramework
{
    /// <summary>
    /// 服务定位器接口 - 提供轻量级的服务解析功能
    /// 替代过度复杂的依赖注入
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// 注册服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="instance">服务实例</param>
        void Register<T>(T instance) where T : class;
        
        /// <summary>
        /// 注册服务类型映射
        /// </summary>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TImplementation">实现类型</typeparam>
        void Register<TInterface, TImplementation>() where TImplementation : TInterface, new();
        
        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例</returns>
        T Resolve<T>() where T : class;
        
        /// <summary>
        /// 解析所有服务实例
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>服务实例集合</returns>
        IEnumerable<T> ResolveAll<T>() where T : class;
        
        /// <summary>
        /// 检查服务是否已注册
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <returns>是否已注册</returns>
        bool IsRegistered<T>() where T : class;
        
        /// <summary>
        /// 移除服务注册
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        void Unregister<T>() where T : class;
    }
}
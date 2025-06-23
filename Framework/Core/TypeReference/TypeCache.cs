namespace XIFramework
{
    using System;
    using System.Collections.Generic;

    public static class TypeCache
    {
        private static readonly Dictionary<Type, List<Type>> cache = new Dictionary<Type, List<Type>>();
    
        public static IReadOnlyList<Type> GetTypesDerivedFrom(Type baseType)
        {
            if (cache.TryGetValue(baseType, out var types))
            {
                return types;
            }
        
            types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // 跳过系统程序集
                if (assembly.FullName.StartsWith("System") || 
                    assembly.FullName.StartsWith("mscorlib"))
                {
                    continue;
                }
            
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    continue;
                }
            
                foreach (var type in assemblyTypes)
                {
                    if (baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        types.Add(type);
                    }
                }
            }
        
            cache[baseType] = types;
            return types;
        }
    }
}
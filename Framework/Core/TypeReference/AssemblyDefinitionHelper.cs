namespace XIFramework
{
#if UNITY_EDITOR
    using UnityEditor;
    using System.Linq;
    using System.Reflection;

    public static class AssemblyDefinitionHelper
    {
        // 获取所有用户程序集（排除Unity系统程序集）
        public static System.Reflection.Assembly[] GetAllUserAssemblies()
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => 
                    !asm.FullName.StartsWith("System") &&
                    !asm.FullName.StartsWith("mscorlib") &&
                    !asm.FullName.StartsWith("netstandard") &&
                    !asm.FullName.StartsWith("UnityEngine") &&
                    !asm.FullName.StartsWith("UnityEditor") &&
                    !asm.FullName.StartsWith("nunit.framework"))
                .ToArray();
        }

        // 获取所有程序集定义名称
        public static string[] GetAllAssemblyNames()
        {
            return GetAllUserAssemblies()
                .Select(asm => asm.GetName().Name)
                .Distinct()
                .OrderBy(name => name)
                .ToArray();
        }

        // 从程序集加载所有类型
        public static System.Type[] GetTypesFromAssembly(string assemblyName)
        {
            var assembly = GetAllUserAssemblies()
                .FirstOrDefault(asm => asm.GetName().Name == assemblyName);
        
            return assembly?.GetTypes() ?? new System.Type[0];
        }
    }
#endif
}
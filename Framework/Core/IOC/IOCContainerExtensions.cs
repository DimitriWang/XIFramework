using System.Collections.Generic;
using System.Linq;

namespace XIFramework
{
    public static class IOCContainerExtensions
    {
        public static IEnumerable<T> ResolveAll<T>(this IXIFrameworkContainer container) where T : class
        {
            // 简化实现，实际项目中可能需要更复杂的逻辑
            try
            {
                return new List<T> { container.Resolve<T>() };
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
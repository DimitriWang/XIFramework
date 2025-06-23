using System;
using System.Collections.Generic;

namespace XIFramework
{
    public class XMemoryAllocatorManager
    {
        public static readonly List<IXIMemoryAllocator> GlobalList = new List<IXIMemoryAllocator>();
        public static void ClearList()
        {
            GlobalList.Clear();
        }
        public static void RegisterClear(Action execute) { }
    }
    public interface IXIMemoryAllocator
    {
            
    }
    
    public class XIMemoryAllocator<T> : IXIMemoryAllocator where T : new() { }
}
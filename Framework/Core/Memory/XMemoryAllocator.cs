using System;
using System.Collections.Generic;

namespace XIFramework
{
    public class XIMemoryAllocatorManager
    {
        public static readonly List<IXIMemoryAllocator> GlobalList = new List<IXIMemoryAllocator>();
        public static void ClearList()
        {
            GlobalList.Clear();
        }
    }
    public interface IXIMemoryAllocator
    {
        string Type { get; }
        string Name { get; }
        int UsedCount { get; }
        int FreeCount { get; }
    }

    public class XIMemoryAllocator<T> : IXIMemoryAllocator where T : new()
    {
        public string Type { get; }
        public string Name { get; }
        public int UsedCount => _usedCount;
        public int FreeCount => _freeList.Count;
        
        private readonly Func<T> AllocMethod;
        private readonly Action<T> ResetMethod;
        private readonly Action<T> RecycleMethod;
        private readonly List<T> _freeList = new List<T>();
        private int _usedCount;
        
        public XIMemoryAllocator(
            Func<T> alloc , 
            string type, 
            string name = "", 
            Action<T> resetMethod = null, 
            Action<T> recycleMethod = null)
        {
            AllocMethod = alloc ?? (() => new T());
            Type = type;
            Name = name;
            ResetMethod = resetMethod;
            RecycleMethod = recycleMethod;
            XIMemoryAllocatorManager.GlobalList.Add(this);
        }

        public bool TryAlloc(out T result)
        {
            _usedCount++;
            if (_freeList.Count > 0)
            {
                result = _freeList[_freeList.Count - 1];
                _freeList.RemoveAt(_freeList.Count - 1);
                ResetMethod?.Invoke(result);
                return true;
            }
            result = AllocMethod();
            return false;
        }
    }
}
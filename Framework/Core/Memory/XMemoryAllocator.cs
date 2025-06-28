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
        
        public static void PrintMemCache(Action<string> logHandler)
        {
            var output = new System.Text.StringBuilder();
            foreach (var allocator in XIMemoryAllocatorManager.GlobalList)
            {
                if (allocator.FreeCount + allocator.UsedCount > 0)
                {
                    output.AppendLine($"{allocator.Name}<{allocator.Type}> UseCount:{allocator.UsedCount}/FreeCount:{allocator.FreeCount}");
                }
            }
            logHandler?.Invoke(output.ToString());
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

        public T Alloc()
        {
            _usedCount++;
            if (_freeList.Count > 0)
            {
                var result = _freeList[_freeList.Count - 1];
                _freeList.RemoveAt(_freeList.Count - 1);
                ResetMethod?.Invoke(result);
                return result;
            }
            return AllocMethod();
        }

        public void Recycle(T obj)
        {
            RecycleMethod?.Invoke(obj);
            if (_freeList.Contains(obj)) return;
            _usedCount--;
            if (_usedCount < 0) _usedCount = 0;
            _freeList.Add(obj);
        }

        public void FreeList(List<T> list, bool clear)
        {
            foreach (var node in list)
            {
                RecycleMethod?.Invoke(node);
            }
            _freeList.AddRange(list);
            _usedCount -= list.Count;
            if (clear) list.Clear();
        }
        
        public void Clear(Action<T> clearAction = null)
        {
            if (clearAction != null)
            {
                foreach (var item in _freeList)
                {
                    clearAction(item);
                }
            }
            _freeList.Clear();
        }
        
        // 新增静态打印方法
        public static void PrintMemCache(Action<string> logHandler)
        {
            XIMemoryAllocatorManager.PrintMemCache(logHandler);
        }

    }
}
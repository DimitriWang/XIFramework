using System;

namespace XIFramework
{
    public class MonoSingletonPathAttribute : Attribute
    {
        public MonoSingletonPathAttribute(string pathInHierarchy)
        {
            PathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy { get; private set; }
    }
}
using System;

namespace XIFramework
{
// InjectAttribute.cs
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute { }

}
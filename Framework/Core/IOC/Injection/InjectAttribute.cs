using System;

namespace XIFramework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute { }

}
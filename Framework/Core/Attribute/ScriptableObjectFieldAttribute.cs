using System;
using UnityEngine;

namespace XIFramework
{
    public class ScriptableObjectFieldAttribute : PropertyAttribute
    {
        public readonly Type Type;
        public ScriptableObjectFieldAttribute(Type type)
        {
            Type = type;
        }
    }
}
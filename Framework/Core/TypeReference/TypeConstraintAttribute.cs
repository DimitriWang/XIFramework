using UnityEngine;

namespace XIFramework
{
    using System;

    using UnityEngine;

    public class TypeConstraintAttribute : PropertyAttribute
    {
        public System.Type BaseType { get; }
        public bool AllowAbstract { get; set; } = false;
        public bool IncludeEditorAssemblies { get; set; } = false;
    
        public TypeConstraintAttribute(System.Type baseType)
        {
            BaseType = baseType;
        }
    }
}
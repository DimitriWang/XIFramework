using System;
using UnityEngine;

namespace XIFramework
{
    using System;
    using UnityEngine;

    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        [SerializeField] 
        private string _assemblyQualifiedName;
    
        [NonSerialized]
        private Type _cachedType;
    
        public Type Type
        {
            get
            {
                if (_cachedType == null && !string.IsNullOrEmpty(_assemblyQualifiedName))
                {
                    _cachedType = Type.GetType(_assemblyQualifiedName);
                }
                return _cachedType;
            }
            set
            {
                _cachedType = value;
                _assemblyQualifiedName = value?.AssemblyQualifiedName;
            }
        }
    
        public void OnBeforeSerialize() { }
    
        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(_assemblyQualifiedName))
            {
                _cachedType = null;
            }
        }
    
        public static implicit operator Type(TypeReference typeReference) => typeReference?.Type;
        public static implicit operator TypeReference(Type type) => new TypeReference { Type = type };
    
        public override string ToString()
        {
            return Type?.Name ?? "None";
        }
    }
}
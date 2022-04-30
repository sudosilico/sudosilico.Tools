using System;
using UnityEngine;

namespace sudosilico.Tools.TypeReferences
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        [SerializeField] 
        private string _assemblyQualifiedTypeName;

        [SerializeField] 
        private string _typeName;
        
        public Type GetReferencedType()
        {
            return _assemblyQualifiedTypeName != null
                       ? Type.GetType(_assemblyQualifiedTypeName)
                       : null;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
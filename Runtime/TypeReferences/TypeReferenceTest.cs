using Sirenix.OdinInspector;
using UnityEngine;

namespace sudosilico.Tools.TypeReferences
{
    public class TypeReferenceTest : MonoBehaviour
    {
        public TypeReference TypeReference1;
        public TypeReference TypeReference2;
        public TypeReference TypeReference3;

        private void Awake()
        {
        }

        [Button("Test1")]
        private void Test1()
        {
        }
        
        [Button("Test2")]
        private void Test2()
        {
        }
    }
}
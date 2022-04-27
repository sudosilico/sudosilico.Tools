using System;
using UnityEngine;

namespace sudosilico.Tools
{
    [Serializable]
    public class UniqueIDReference : ISerializationCallbackReceiver
    {
        public UniqueID TargetID;
        public SceneReference ContainingScene;
        
#if UNITY_EDITOR
        [SerializeField]
        private GameObject _cachedGameObject;

        [SerializeField]
        private string _cachedName;
#endif
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            _cachedGameObject = null;
        }
    }
}
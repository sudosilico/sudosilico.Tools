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
        // ReSharper disable once NotAccessedField.Local
        private GameObject _cachedGameObject;

        [SerializeField]
        // ReSharper disable once NotAccessedField.Local
        private string _cachedName;
#endif

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            _cachedGameObject = null;
#endif
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sudosilico.Tools
{
    public static class UniqueIDManager
    {
        public static readonly Dictionary<UniqueID, UniqueIDComponent> Components = new();

        public static event Action<UniqueID> OnComponentAdded = delegate(UniqueID id) {  };
        public static event Action<UniqueID> OnComponentRemoved = delegate(UniqueID id) {  };
        
        /// <summary>
        /// Attempts to add the given UniqueID component to the UniqueID manager, returning a value
        /// indicating whether it succeeded.
        /// This will return false if the UniqueID is already present in the manager.
        /// This will throw if the UniqueID is null or empty.
        /// </summary>
        public static bool AddComponent(UniqueID uniqueID, UniqueIDComponent component)
        {
            if (uniqueID == null)
            {
                throw new ArgumentNullException(nameof(uniqueID));
            }
            
            if (uniqueID.IsEmpty())
            {
                throw new ArgumentException("UniqueID may not be empty.");
            }

            if (Components.ContainsKey(uniqueID))
            {
                return false;
            }

            Components.Add(uniqueID, component);
            OnComponentAdded?.Invoke(uniqueID);

            return true;
        }

        public static bool RemoveComponent(UniqueID uniqueID)
        {
            if (Components.Remove(uniqueID))
            {
                OnComponentRemoved?.Invoke(uniqueID);
                return true;
            }
            
            return false;
        }

        public static GameObject GetGameObject(UniqueID id)
        {
            if (Components.TryGetValue(id, out var component))
                return component.gameObject;

            return null;
        }

#if UNITY_EDITOR
        public class UniqueIDManagerAssetModificationProcessor : AssetModificationProcessor
        {
            private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
            {
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(sourcePath);

                if (assetType == typeof(SceneAsset))
                {
                    // Renaming a scene.
                    Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
                    Debug.Log("Type: " + assetType.FullName);    
                }

                return AssetMoveResult.DidNotMove;
            }
        }
#endif
    }
}
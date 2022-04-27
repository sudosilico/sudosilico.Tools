using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace sudosilico.Tools
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class UniqueIDComponent : MonoBehaviour, ISerializationCallbackReceiver
    {
        public UniqueID ID;

        public void Awake()
        {
            InitializeUniqueID();
        }

        private void InitializeUniqueID()
        {
            if (ID == null || ID.IsEmpty())
            {
                RegenerateGUID();
            }
        }

        private void OnEnable()
        {
            if (ID == null || ID.IsEmpty())
            {
                return;
            }

            bool added = UniqueIDManager.AddComponent(ID, this);
                
            if (!added)
            {
                ID = null;
                InitializeUniqueID();
                    
                bool newIDAdded = UniqueIDManager.AddComponent(ID, this);
                
                if (!newIDAdded)
                {
                    Debug.LogError("Couldn't generate a new ID for this object: " + gameObject.name);
                }
            }
        }

        private void OnDisable()
        {
            UniqueIDManager.RemoveComponent(ID);
        }
        
        private void RegenerateGUID()
        {
#if UNITY_EDITOR
            // don't generate an ID for a prefab
            if (IsAssetOnDisk())
            {
                return;
            }

            Undo.RecordObject(this, "Added UniqueID");
#endif
                
            ID = UniqueID.Generate();
                
#if UNITY_EDITOR
            // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
            // force a save of the modified prefab instance properties
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
#endif
        }
        
#if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if (EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }

            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gameObject);
            
            if (currentStage != mainStage)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                if (prefabStage != null)
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool IsAssetOnDisk()
        {
            return PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
        }
#endif
        
        void OnValidate()
        {
#if UNITY_EDITOR
            // prefabs can't have IDs
            if (IsAssetOnDisk())
            {
                ID = null;
            }
            else
#endif
            {
                InitializeUniqueID();
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // prefabs can't have IDs
            if (IsAssetOnDisk())
            {
                ID = null;
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
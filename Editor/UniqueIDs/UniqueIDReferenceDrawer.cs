using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace sudosilico.Tools
{
    [CustomPropertyDrawer(typeof(UniqueIDReference))]
    public class UniqueIDReferenceDrawer : PropertyDrawer
    {
        private static GUIContent _redIcon = EditorGUIUtility.IconContent("d_winbtn_mac_close");
        private static GUIContent _orangeIcon = EditorGUIUtility.IconContent("d_winbtn_mac_min");
        
        private byte[] emptyBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] guidBytes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sceneRefProp = property.FindPropertyRelative("ContainingScene");
            var sceneAssetProp = sceneRefProp.FindPropertyRelative("_sceneAsset");
            
            if (sceneAssetProp.objectReferenceValue != null)
            {
                return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + 2;
            }

            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            
            #region Props 
            
            var idProp = property.FindPropertyRelative("TargetID");
            var serializedGuidProp = idProp?.FindPropertyRelative("_serializedGuid");
            var sceneRefProp = property.FindPropertyRelative("ContainingScene");
            var sceneAssetProp = sceneRefProp.FindPropertyRelative("_sceneAsset");
            var sceneAssetPathProp = sceneRefProp.FindPropertyRelative("_sceneAssetPath");
            var sceneNameProp = sceneRefProp.FindPropertyRelative("_sceneName");
            var sceneIndexProp = sceneRefProp.FindPropertyRelative("_sceneIndex");
            var sceneEnabledProp = sceneRefProp.FindPropertyRelative("_sceneEnabled");
            var cachedGOProp = property.FindPropertyRelative("_cachedGameObject");
            var cachedNameProp = property.FindPropertyRelative("_cachedName");

            #endregion
            
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            UniqueID currentID = ParseByteArrayProp(serializedGuidProp);
            GameObject currentGO = (!currentID.IsEmpty()) ? UniqueIDManager.GetGameObject(currentID) : null;

            #region Object Field

            void ClearSerializedReference()
            {
                // Clear id
                SetByteArrayProp(serializedGuidProp, emptyBytes.ToArray());

                // Clear scene
                sceneAssetPathProp.stringValue = null;
                sceneNameProp.stringValue = null;
                sceneIndexProp.intValue = -1;
                sceneEnabledProp.boolValue = false;
                sceneAssetProp.objectReferenceValue = null;

                // Clear cached 
                cachedGOProp.objectReferenceValue = null;
                cachedNameProp.stringValue = null;
            }

            void HandleObjectSelection(Object o)
            {
                if (o is GameObject go)
                {
                    var prefabType = PrefabUtility.GetPrefabAssetType(go);

                    if (prefabType == PrefabAssetType.NotAPrefab)
                    {
                        // Make sure the selected object has a UniqueIDComponent on it.
                        var idComponent = go.GetComponent<UniqueIDComponent>();
                        if (idComponent == null)
                        {
                            idComponent = go.AddComponent<UniqueIDComponent>();
                        }

                        // Save id
                        SetByteArrayProp(serializedGuidProp, idComponent.ID.GetBytes());

                        var scene = go.scene;

                        // Save scene
                        sceneAssetPathProp.stringValue = scene.path;
                        sceneNameProp.stringValue = scene.name;
                        sceneIndexProp.intValue = scene.buildIndex;
                        sceneEnabledProp.boolValue = scene.buildIndex != -1; // TODO: Check if this is correct
                        sceneAssetProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);

                        // Save cached 
                        cachedGOProp.objectReferenceValue = go;
                        cachedNameProp.stringValue = go.name;
                    }
                    else
                    {
                        Debug.LogError("A UniqueIDReference cannot reference a prefab: use 'GameObject' instead.");
                        ClearSerializedReference();
                    }
                }
                else
                {
                    ClearSerializedReference();
                }
            }

            if (currentID.IsEmpty())
            {
                var labelPos = EditorGUI.PrefixLabel(position, label);
                
                // Nothing is set

                EditorGUI.BeginChangeCheck();
                var selection = EditorGUI.ObjectField(labelPos,
                                                      GUIContent.none,
                                                      null,
                                                      typeof(GameObject),
                                                      true);

                if (EditorGUI.EndChangeCheck())
                {
                    HandleObjectSelection(selection);
                }
            }
            else
            {
                var labelPos = EditorGUI.PrefixLabel(position, label);
                
                if (currentGO == null)
                {
                    // if our reference is set, but the target isn't loaded, we display the target and the scene it is in, and provide a way to clear the reference
                    float buttonWidth = 55.0f;

                    labelPos.xMax -= buttonWidth;
                    
                    bool guiEnabled = GUI.enabled;
                    GUI.enabled = false;
                    EditorGUI.LabelField(labelPos,
                                         new GUIContent(cachedNameProp.stringValue, "Object is not loaded."),
                                         EditorStyles.objectField);
                    GUI.enabled = guiEnabled;

                    var buttonRect = new Rect(labelPos);
                    buttonRect.xMin = buttonRect.xMax;
                    buttonRect.xMax += buttonWidth;

                    if (GUI.Button(buttonRect, new GUIContent("Clear", "Remove Cross Scene Reference"), EditorStyles.miniButton))
                    {
                        ClearSerializedReference();
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    var selection = EditorGUI.ObjectField(labelPos,
                                                          GUIContent.none,
                                                          currentGO,
                                                          typeof(GameObject),
                                                          true);

                    if (EditorGUI.EndChangeCheck())
                    {
                        HandleObjectSelection(selection);
                    }
                }
            }

            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            if (sceneAssetProp.objectReferenceValue != null)
            {
                EditorGUI.indentLevel++;
                
                bool guiEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.ObjectField(position,
                                      new GUIContent("Containing Scene", "The target object is expected in this scene asset."),
                                      sceneAssetProp.objectReferenceValue,
                                      typeof(SceneAsset),
                                      false);
                GUI.enabled = guiEnabled;
                
                EditorGUI.indentLevel--;
            }

            #endregion

            EditorGUI.EndProperty();
        }

        private UniqueID ParseByteArrayProp(SerializedProperty guidProp)
        {
            for (int i = 0; i < 16; i++)
            {
                guidBytes[i] = (byte)guidProp.GetArrayElementAtIndex(i).intValue;
            }

            return new UniqueID(new Guid(guidBytes));
        }

        private void SetByteArrayProp(SerializedProperty guidProp, byte[] bytes)
        {
            for (int i = 0; i < 16; i++)
            {
                guidProp.GetArrayElementAtIndex(i).intValue = bytes[i];
            }
        }
    }
}
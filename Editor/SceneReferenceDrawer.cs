using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace sudosilico.Tools
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        private static GUIContent _redIcon = EditorGUIUtility.IconContent("d_winbtn_mac_close");
        private static GUIContent _orangeIcon = EditorGUIUtility.IconContent("d_winbtn_mac_min");
        
        private readonly string[] popupOptionsNotInBuildList = { "Add to build list..." };
        private readonly string[] popupOptionsDisabled = { "Enable Scene" };
        private readonly string[] popupOptionsEnabledPlayMode = { "Load Scene", "Load Scene Async" };
        private readonly string[] popupOptionsEnabled = { "Open Scene" };

        private string _pendingAssetToOpen;
        private GUIStyle popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            popupStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };
            
            var sceneProp = property.FindPropertyRelative("_sceneAsset");

            var scenes = EditorBuildSettings.scenes;

            string assetPath = AssetDatabase.GetAssetPath(sceneProp.objectReferenceValue);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            EditorBuildSettingsScene buildScene = scenes.FirstOrDefault(scene => scene.guid.ToString() == assetGUID);

            var sceneStatus = GetSceneStatus(sceneProp.objectReferenceValue, buildScene);
            
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var labelContent = new GUIContent();
            labelContent = GetIcon(sceneStatus, labelContent);
            labelContent.text = label.text;

            position = EditorGUI.PrefixLabel(position, labelContent);
            
            bool drawDropdown = sceneStatus != SceneFieldStatus.Null;
            
            if (drawDropdown)
            {
                // Calculate rect for configuration button
                Rect buttonRect = new Rect(position);
                buttonRect.yMin += popupStyle.margin.top;
                buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
                position.xMin = buttonRect.xMax;
            
                // options dropdown
                EditorGUI.BeginChangeCheck();
                int result = EditorGUI.Popup(buttonRect, -1, GetPopupOptions(sceneStatus), popupStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    HandlePopupSelection(sceneStatus, result, assetPath, assetGUID, buildScene);
                }
            }
            
            EditorGUI.BeginChangeCheck();
            Object selectedObject = EditorGUI.ObjectField(position, GUIContent.none, sceneProp.objectReferenceValue, typeof(SceneAsset), false);
            
            if (EditorGUI.EndChangeCheck())
            {
                sceneProp.objectReferenceValue = selectedObject;
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private string[] GetPopupOptions(SceneFieldStatus status)
        {
            switch (status)
            {
                case SceneFieldStatus.Null:
                    return new string[] { };
                case SceneFieldStatus.NotInBuildList:
                    return popupOptionsNotInBuildList;
                case SceneFieldStatus.Disabled:
                    return popupOptionsDisabled;
                case SceneFieldStatus.Enabled:
                    return Application.isPlaying ? popupOptionsEnabledPlayMode : popupOptionsEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void HandlePopupSelection(SceneFieldStatus status, int index, string assetPath, string assetGUID, EditorBuildSettingsScene buildScene)
        {
            switch (status)
            {
                case SceneFieldStatus.Null:
                    break;
                case SceneFieldStatus.NotInBuildList:
                    if (index == 0)
                    {
                        AddBuildScene(assetPath, assetGUID);
                    }
                    break;
                case SceneFieldStatus.Disabled:
                    if (index == 0)
                    {
                        EnableScene(assetGUID);
                    }
                    break;
                case SceneFieldStatus.Enabled:
                    switch (index)
                    {
                        case 0:
                        {
                            if (Application.isPlaying)
                            {
                                var loadedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                                if (loadedSceneAsset == null)
                                {
                                    Debug.LogError("Could not find a scene at path: " + assetPath);
                                }
                                else
                                {
                                    SceneManager.LoadScene(loadedSceneAsset.name);
                                }
                            }
                            else
                            {
                                OpenSceneAtPath(assetPath);
                            }

                            break;
                        }
                        case 1:
                        {
                            if (Application.isPlaying)
                            {
                                var loadedSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                                if (loadedSceneAsset == null)
                                {
                                    Debug.LogError("Could not find a scene at path: " + assetPath);
                                }
                                else
                                {
                                    SceneManager.LoadSceneAsync(loadedSceneAsset.name);     
                                }
                            }
                            else
                            {
                                OpenSceneAtPath(assetPath);
                            }

                            break;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        private void OpenSceneAtPath(string assetPath)
        {
            _pendingAssetToOpen = assetPath;
            
            // Because some of our UI code happens after we handle dropdown clicks, there will be references to
            // disposed objects if we open the scene immediately then, so we defer it to the next update.
            
            EditorApplication.update += OpenSceneAtPath_OnEditorUpdate;
        }

        private void OpenSceneAtPath_OnEditorUpdate()
        {
            EditorApplication.update -= OpenSceneAtPath_OnEditorUpdate;
            
            if (_pendingAssetToOpen == null) 
                return;

            // OpenScene on its own will discard changes, so we ask the user to save first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // Selected 'cancel'
                return;
            }
            
            // Saved or selected 'Don't save'
            
            EditorSceneManager.OpenScene(_pendingAssetToOpen);
            _pendingAssetToOpen = null;
        }

        private GUIContent GetIcon(SceneFieldStatus status, GUIContent content)
        {
            switch (status)
            {
                case SceneFieldStatus.Null:
                    content.image = _redIcon.image;
                    content.tooltip = "Scene is null.";
                    break;
                case SceneFieldStatus.NotInBuildList:
                    content.image = _orangeIcon.image;
                    content.tooltip = "Scene is not in build list.";
                    break;
                case SceneFieldStatus.Disabled:
                    content.image = _orangeIcon.image;
                    content.tooltip = "Scene is disabled.";
                    break;
                case SceneFieldStatus.Enabled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }

            return content;
        }

        private SceneFieldStatus GetSceneStatus(Object sceneAsset, EditorBuildSettingsScene buildScene)
        {
            if (sceneAsset == null)
            {
                return SceneFieldStatus.Null;
            }

            if (buildScene == null)
            {
                return SceneFieldStatus.NotInBuildList;
            }

            return buildScene.enabled ? SceneFieldStatus.Enabled : SceneFieldStatus.Disabled;
        }
        
        public static void AddBuildScene(string assetPath, string assetGUID, bool force = false, bool enabled = true)
        {
            if (force == false)
            {
                int selection = EditorUtility.DisplayDialogComplex(
                                                                   "Add Scene To Build",
                                                                   "You are about to add scene at " + assetPath + " To the Build Settings.",
                                                                   "Add as Enabled",       // option 0
                                                                   "Add as Disabled",      // option 1
                                                                   "Cancel (do nothing)"); // option 2

                switch (selection)
                {
                    case 0: // enabled
                        enabled = true;
                        break;
                    case 1: // disabled
                        enabled = false;
                        break;
                    default:
                    case 2: // cancel
                        return;
                }
            }

            EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(new GUID(assetGUID), enabled);
            var scenes = EditorBuildSettings.scenes.ToList();
            scenes.Add(newScene);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        public static void EnableScene(string assetGUID)
        {
            var modifiedScenes = EditorBuildSettings.scenes;

            foreach (var scene in modifiedScenes)
            {
                if (scene.guid.ToString().Equals(assetGUID))
                {
                    scene.enabled = true;
                }
            }

            EditorBuildSettings.scenes = modifiedScenes;          
        }
        
        private enum SceneFieldStatus
        {
            Null,
            NotInBuildList,
            Disabled,
            Enabled,
        }
    }
}
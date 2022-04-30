using sudosilico.Tools.GameEvents;
using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.RuntimeSets
{
    [CustomEditor(typeof(RuntimeSet), editorForChildClasses: true)]
    public class RuntimeSetEditor : Editor
    {
        private const string _createButtonTooltip = "Create a GameEvent, assign it to OnChange, and add it to the RuntimeSet asset.";
        private const string _clearButtonTooltip = "";

        private static bool _stylesLoaded;
        private static GUIStyle _buttonStyle;
        private static GUILayoutOption _buttonWidth;
        private static GUIContent _createButtonContent;
        private static GUIContent _clearButtonContent;

        private void OnEnable()
        {

        }

        private void LoadStyles()
        {
            _buttonStyle ??= new(GUI.skin.button);
            _buttonWidth ??= GUILayout.MaxWidth(80);
            _createButtonContent ??= new("Create", _createButtonTooltip);
            _clearButtonContent ??= new("Clear", _clearButtonTooltip);
            _stylesLoaded = true;
        }
        
        public override void OnInspectorGUI()
        {
            if (!_stylesLoaded) 
                LoadStyles();
            
            var runtimeSet = target as RuntimeSet;
            if (runtimeSet == null) 
                return;
            
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            
            var onChange = runtimeSet.OnChange;
            
            GUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            runtimeSet.OnChange = EditorGUILayout.ObjectField(new GUIContent("OnChange", ""),
                                                              onChange,
                                                              typeof(GameEvent),
                                                              false) as GameEvent;
            EditorGUI.EndDisabledGroup();
            
            if (onChange == null)
            {
                if (GUILayout.Button(_createButtonContent, _buttonStyle, _buttonWidth))
                {
                    var newGameEvent = CreateInstance<GameEvent>();
                    newGameEvent.name = $"{runtimeSet.name}.OnChange";

                    AssetDatabase.AddObjectToAsset(newGameEvent, runtimeSet);

                    runtimeSet.OnChange = newGameEvent;
                    EditorUtility.SetDirty(runtimeSet);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                if (GUILayout.Button(_clearButtonContent, _buttonStyle, _buttonWidth))
                {
                    runtimeSet.OnChange = null;
                        
                    AssetDatabase.RemoveObjectFromAsset(onChange);
                    DestroyImmediate(onChange);
                        
                    EditorUtility.SetDirty(runtimeSet);
                    AssetDatabase.SaveAssets();
                }
            }
            
            GUILayout.EndHorizontal();
        }
    }
}
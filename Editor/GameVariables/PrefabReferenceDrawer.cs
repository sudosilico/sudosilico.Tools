using System;
using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CustomPropertyDrawer(typeof(PrefabReference))]
    public class PrefabReferenceDrawer : PropertyDrawer
    {
        private static readonly string[] _popupOptions = { "Value", "Variable" };
        private static GUIStyle _popupStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get properties
            SerializedProperty useConstant = property.FindPropertyRelative("_useValue");
            SerializedProperty constantValue = property.FindPropertyRelative("_value");
            SerializedProperty variable = property.FindPropertyRelative("_variable");
            
            _popupStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            // Calculate rect for configuration button
            Rect buttonRect = new Rect(position);
            buttonRect.yMin += _popupStyle.margin.top;
            buttonRect.width = _popupStyle.fixedWidth + _popupStyle.margin.right;
            position.xMin = buttonRect.xMax;

            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            bool shouldUseConstant = useConstant.boolValue;
            
            EditorGUI.BeginChangeCheck();
            
            int result = EditorGUI.Popup(buttonRect, shouldUseConstant ? 0 : 1, _popupOptions, _popupStyle);

            useConstant.boolValue = result == 0;

            var oldValue = useConstant.boolValue ? constantValue.objectReferenceValue : variable.objectReferenceValue;
            
            // Only prefabs
            var newValue = EditorGUI.ObjectField(position, oldValue, typeof(GameObject), false) as GameObject;
            
            if (useConstant.boolValue)
            {
                constantValue.objectReferenceValue = newValue;
            }
            else
            {
                variable.objectReferenceValue = newValue;
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
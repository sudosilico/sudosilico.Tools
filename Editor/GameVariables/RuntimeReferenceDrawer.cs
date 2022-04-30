using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    /// <summary>
    /// Handles drawing for VariableReference properties whose values are not serialized.
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectReference))]
    [CustomPropertyDrawer(typeof(TransformReference))]
    public class RuntimeReferenceDrawer : PropertyDrawer
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

            if (useConstant.boolValue)
            {
                // Should be disabled, because it can only have a value at runtime.
                EditorGUI.BeginDisabledGroup(true);

                EditorGUI.PropertyField(position, 
                                        constantValue, 
                                        GUIContent.none);
                
                EditorGUI.LabelField(position, new GUIContent("", "Values of this type can only exist at runtime."));
                
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.PropertyField(position, 
                                        variable, 
                                        GUIContent.none);    
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
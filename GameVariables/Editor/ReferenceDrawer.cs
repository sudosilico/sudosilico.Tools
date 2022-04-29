using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    /// <summary>
    /// Handles drawing for VariableReference properties whose values may be serialized.
    /// </summary>
    [CustomPropertyDrawer(typeof(BoolReference))]
    [CustomPropertyDrawer(typeof(ColorReference))]
    [CustomPropertyDrawer(typeof(Color32Reference))]
    [CustomPropertyDrawer(typeof(FloatReference))]
    [CustomPropertyDrawer(typeof(IntReference))]
    [CustomPropertyDrawer(typeof(StringReference))]
    [CustomPropertyDrawer(typeof(Vector2Reference))]
    [CustomPropertyDrawer(typeof(Vector3Reference))]
    [CustomPropertyDrawer(typeof(Vector4Reference))]
    public class ReferenceDrawer : PropertyDrawer
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

            EditorGUI.PropertyField(position, 
                                    useConstant.boolValue ? constantValue : variable, 
                                    GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
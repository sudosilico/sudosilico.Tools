using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace sudosilico.Tools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PrefabAttribute : PropertyAttribute
    {
        public Type ComponentFilterType;

        public PrefabAttribute()
        {
        }
        
        public PrefabAttribute(Type componentFilterType)
        {
            ComponentFilterType = componentFilterType;
        }
    }
    
    #if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(PrefabAttribute))]
    public class PrefabPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            EditorGUI.BeginChangeCheck();

            var currentValue = property.objectReferenceValue;

            var selectedPrefab = EditorGUI.ObjectField(position, currentValue, typeof(GameObject), false) as GameObject;
            
            property.objectReferenceValue = selectedPrefab;
            
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }
    }
    
    #endif
}
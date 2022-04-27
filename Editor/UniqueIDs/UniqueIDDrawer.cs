using System;
using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools
{
    [CustomPropertyDrawer(typeof(UniqueID))]
    public class UniqueIDDrawer : PropertyDrawer
    {
        private byte[] guidBytes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty guidProp = property.FindPropertyRelative("_serializedGuid");

            for (int i = 0; i < 16; i++)
            {
                guidBytes[i] = (byte)guidProp.GetArrayElementAtIndex(i).intValue;
            }

            EditorGUI.LabelField(position, label, new GUIContent(new Guid(guidBytes).ToString()));
        }
    }
}
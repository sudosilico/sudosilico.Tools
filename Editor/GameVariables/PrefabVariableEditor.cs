using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.GameVariables
{
    [CustomEditor(typeof(PrefabVariable))]
    public class PrefabVariableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PrefabVariable prefabVariable = target as PrefabVariable;

            if (prefabVariable != null)
            {
                prefabVariable.Value = EditorGUILayout.ObjectField("Prefab to search for",
                                                                   prefabVariable.Value,
                                                                   typeof(GameObject),
                                                                   false)
                                           as GameObject;
            }
        }
    }
}
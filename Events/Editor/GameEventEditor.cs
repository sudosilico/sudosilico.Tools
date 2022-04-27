using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools.Events
{
    [CustomEditor(typeof(GameEvent), editorForChildClasses: true)]
    public class GameEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEvent e = target as GameEvent;
            
            if (GUILayout.Button("Raise") && e != null)
            {
                e.Raise();
            }
        }
    }
}
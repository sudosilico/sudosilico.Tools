using System.IO;
using UnityEditor;
using UnityEngine;

namespace sudosilico.Tools
{
    public static class Folders
    {
        public static void EnsureProjectFoldersExist()
        {
            string projectPath = Path.Combine(Application.dataPath, "_Project");

            if (EditorUtility.DisplayDialog(
                                        "Create _Project folders...", 
                                        "Are you sure you want to create _Project folders?", 
                                        "OK", 
                                        "Cancel"))
            {
                Dir(projectPath);
                Dir(projectPath, "Art");
                Dir(projectPath, "Audio");
                Dir(projectPath, "Audio", "Music");
                Dir(projectPath, "Audio", "Sounds");
                Dir(projectPath, "Data");
                Dir(projectPath, "Scenes");
                Dir(projectPath, "Scenes", "Gameplay");
                Dir(projectPath, "Scenes", "Menu");
                Dir(projectPath, "Scenes", "Test");
                Dir(projectPath, "Scripts");
            }
        }
        
        private static void Dir(params string[] pathParts)
        {
            var path = Path.Combine(pathParts);
                
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
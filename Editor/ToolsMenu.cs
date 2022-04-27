using UnityEditor;

namespace sudosilico.Tools
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        private static void CreateDefaultFolders()
        {
            Folders.EnsureProjectFoldersExist();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/Setup/Replace Package Manifest")]
        private static void ReplacePackageManifest()
        {
            Folders.EnsureProjectFoldersExist();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Setup/Packages/Input System")]
        private static void AddInputSystem() => Packages.InstallUnityPackage("inputsystem");
        
        [MenuItem("Tools/Setup/Packages/Post Processing")]
        private static void AddPostProcessing() => Packages.InstallUnityPackage("postprocessing");
        
        [MenuItem("Tools/Setup/Packages/Cinemachine")]
        private static void AddCinemachine() => Packages.InstallUnityPackage("cinemachine");
        
        [MenuItem("Tools/Setup/Packages/Burst Compiler")]
        private static void AddBurstCompiler() => Packages.InstallUnityPackage("burst");
        
        [MenuItem("Tools/Setup/Packages/ProBuilder")]
        private static void AddProBuilder() => Packages.InstallUnityPackage("probuilder");
        
        [MenuItem("Tools/Setup/Packages/Shader Graph")]
        private static void AddShaderGraph() => Packages.InstallUnityPackage("shadergraph");
    }    
}

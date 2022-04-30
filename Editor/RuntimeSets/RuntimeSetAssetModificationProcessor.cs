using System.IO;
using UnityEditor;

namespace sudosilico.Tools.RuntimeSets
{
    public class RuntimeSetAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(sourcePath);
            
            // If we're renaming a RuntimeSet, update its event names to match.
            if (typeof(RuntimeSet).IsAssignableFrom(assetType))
            {
                var runtimeSet = AssetDatabase.LoadAssetAtPath<RuntimeSet>(sourcePath);

                if (runtimeSet != null)
                {
                    string newName = Path.GetFileNameWithoutExtension(destinationPath);

                    if (runtimeSet.OnChange != null)
                    {
                        runtimeSet.OnChange.name = $"{newName}.OnChange";
                    }
                    
                    EditorUtility.SetDirty(runtimeSet);
                    AssetDatabase.SaveAssets();
                }
            }

            return AssetMoveResult.DidNotMove;
        }
    }
}
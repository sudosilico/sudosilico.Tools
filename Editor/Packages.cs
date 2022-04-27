using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace sudosilico.Tools
{
    public static class Packages
    {
        public static void InstallUnityPackage(string packageName, string prefix = "com.unity.")
        {
            UnityEditor.PackageManager.Client.Add(prefix == null 
                                                      ? packageName 
                                                      : $"{prefix}{packageName}");
        }
        
        private static string GetGistURL(string id, string user = "sudosilico")
        {
            return $"https://gist.githubusercontent.com/{user}/{id}/raw";
        }

        private static async Task<string> GetURLContents(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public static void ReplacePackageFile(string contents)
        {
            var packagesPath = Path.Combine(Application.dataPath, "../Packages");
            var manifestPath = Path.Combine(packagesPath, "manifest.json");
            var originalPath = Path.Combine(packagesPath, "manifest.orig.json");

            if (File.Exists(originalPath))
            {
                File.Delete(originalPath);
            }
            
            File.Copy(manifestPath, originalPath);
            File.WriteAllText(manifestPath, contents);
        }
    }
}
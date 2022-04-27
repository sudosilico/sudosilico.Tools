using System;
using System.IO;
using System.Linq;
using UnityEngine;

/*
 *  From:
 *  https://github.com/kellygravelyn/UnityToolbag/blob/main/StandardPaths/StandardPaths.cs
 */

namespace sudosilico.Tools
{
    public static class StandardPaths
    {
        private static readonly char[] _invalidPathCharacters = Path.GetInvalidFileNameChars();
        public static bool IncludeCompanyName { get; set; }
        
        public static string SaveDataDirectory
        {
            get
            {
                string path;

                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Saves");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetApplicationSupportPath("Saves");
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetSaveDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string ConfigDirectory
        {
            get
            {
                string path;

                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Config");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetApplicationSupportPath("Config");
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetConfigDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string LogDirectory
        {
            get
            {
                string path;

                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.WindowsPlayer:
                    {
                        path = Windows.GetPath("Logs");
                        break;
                    }
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    {
                        path = OSX.GetLogsPath();
                        break;
                    }
                    case RuntimePlatform.LinuxPlayer:
                    {
                        path = Linux.GetLogDirectory();
                        break;
                    }
                    default:
                    {
                        path = Application.persistentDataPath;
                        break;
                    }
                }

                Directory.CreateDirectory(path);
                return path;
            }
        }

        static StandardPaths()
        {
            IncludeCompanyName = false;
        }

        private static string GetHOME()
        {
            return Environment.GetEnvironmentVariable("HOME")
                   ?? throw new InvalidOperationException("Error reading HOME environment variable.");
        }

        private static string AppendProductPath(string path)
        {
            if (IncludeCompanyName)
            {
                path = AppendDirectory(path, Application.companyName);
            }

            return AppendDirectory(path, Application.productName);
        }

        private static string AppendDirectory(string path, string dir)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return path;
            }

            dir = CleanForPath(dir);
            return Path.Combine(path, dir);
        }

        private static string CleanForPath(string str)
        {
            return _invalidPathCharacters.Aggregate(str, (current, ch) => current.Replace(ch, '_'));
        }

        private static class Windows
        {
            public static string GetPath(string subdirectory)
            {
                string result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                                             "My Games");
                result = AppendProductPath(result);
                return AppendDirectory(result, subdirectory);
            }
        }

        private static class OSX
        {
            public static string GetApplicationSupportPath(string subdirectory)
            {
                string result = Path.Combine(GetHOME(), "Library/Application Support");
                result = AppendProductPath(result);
                return AppendDirectory(result, subdirectory);
            }

            public static string GetLogsPath()
            {
                string result = Path.Combine(GetHOME(), "Library/Logs");
                return AppendProductPath(result);
            }
        }

        private static class Linux
        {
            public static string GetSaveDirectory()
            {
                string result = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (string.IsNullOrEmpty(result))
                {
                    string home = GetHOME();
                    result = Path.Combine(home, ".local/share");
                }

                return AppendProductPath(result);
            }

            public static string GetConfigDirectory()
            {
                string result = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                if (string.IsNullOrEmpty(result))
                {
                    string home = GetHOME();
                    result = Path.Combine(home, ".config");
                }

                return AppendProductPath(result);
            }

            public static string GetLogDirectory()
            {
                return AppendProductPath("/var/log");
            }
        }
    }
}